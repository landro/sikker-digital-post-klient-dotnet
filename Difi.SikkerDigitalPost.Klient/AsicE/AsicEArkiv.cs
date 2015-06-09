﻿/** 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *         http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using Difi.SikkerDigitalPost.Klient.Domene.Entiteter.Interface;
using Difi.SikkerDigitalPost.Klient.Domene.Entiteter.Post;
using Difi.SikkerDigitalPost.Klient.Utilities;
using Ionic.Zip;
using Ionic.Zlib;

namespace Difi.SikkerDigitalPost.Klient.AsicE
{
    internal class AsicEArkiv : ISoapVedlegg
    {
        public Manifest Manifest { get; private set; }
        public Signatur Signatur { get; private set; }
        private readonly Dokumentpakke _dokumentpakke;
        private readonly Forsendelse _forsendelse;

        private byte[] _bytes;
        private byte[] _ukrypterteBytes;

        private readonly GuidHandler _guidHandler;


        public AsicEArkiv(Forsendelse forsendelse, GuidHandler guidHandler, X509Certificate2 avsenderSertifikat)
        {
            Manifest = new Manifest(forsendelse);
            Signatur = new Signatur(forsendelse, Manifest, avsenderSertifikat);

            _forsendelse = forsendelse;
            _dokumentpakke = _forsendelse.Dokumentpakke;
            _guidHandler = guidHandler;
        }

        private X509Certificate2 _krypteringssertifikat 
        {
            get { return _forsendelse.PostInfo.Mottaker.Sertifikat; }
        }
        
        public string Filnavn
        {
            get { return "post.asice.zip"; }
        }

        internal byte[] UkrypterteBytes
        {
            get
            {
                if (_ukrypterteBytes != null)
                    return _ukrypterteBytes;

                _ukrypterteBytes = LagBytes();
                return _ukrypterteBytes;
            }
        }

        public byte[] Bytes
        {
            get
            {
                if (_bytes != null)
                    return _bytes;

                _bytes = KrypterteBytes(UkrypterteBytes);
                return _bytes;
            }
        }

        public string Innholdstype
        {
            get { return "application/cms"; }
        }

        public string ContentId
        {
            get { return _guidHandler.DokumentpakkeId; }
        }

        public string TransferEncoding
        {
            get { return "binary"; }
        }

        private byte[] LagBytes()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (ZipFile zip = new ZipFile())
            {
                zip.AddEntry(_dokumentpakke.Hoveddokument.VasketFilnavn, _dokumentpakke.Hoveddokument.Bytes);
                zip.AddEntry(Manifest.Filnavn, Manifest.Bytes);
                zip.AddEntry(Signatur.Filnavn, Signatur.Bytes);

                foreach (var dokument in _dokumentpakke.Vedlegg)
                {
                    zip.AddEntry(dokument.VasketFilnavn, dokument.Bytes);
                }

                zip.Save(memoryStream);
                return memoryStream.ToArray();
            }
            



            //var stream = new MemoryStream();
            //using (var archive = new ZipPackage(stream, ZipArchiveMode.Create))
            //{
            //    LeggFilTilArkiv(archive, _dokumentpakke.Hoveddokument.VasketFilnavn, _dokumentpakke.Hoveddokument.Bytes);
            //    LeggFilTilArkiv(archive, Manifest.Filnavn, Manifest.Bytes);
            //    LeggFilTilArkiv(archive, Signatur.Filnavn, Signatur.Bytes);

            //    foreach (var dokument in _dokumentpakke.Vedlegg)
            //        LeggFilTilArkiv(archive, dokument.VasketFilnavn, dokument.Bytes);

            //}

            //byte[] ziparray = memor.ToArray();
            //return stream.ToArray();
        }

        public void LagreTilDisk(params string[] filsti)
        {
            FileUtility.WriteToBasePath(UkrypterteBytes, filsti);
        }

        //private void LeggFilTilArkiv(ZipArchive archive, string filename, byte[] data)
        //{
        //    Logging.Log(TraceEventType.Information, Manifest.Forsendelse.KonversasjonsId, string.Format("Legger til '{0}' på {1} bytes til dokumentpakke.", filename, data.Length));

        //    var entry = archive.CreateEntry(filename, CompressionLevel.Optimal);
        //    using (Stream s = entry.Open())
        //    {
        //        s.Write(data, 0, data.Length);
        //        s.Close();
        //    }
        //}

        private byte[] KrypterteBytes(byte[] bytes)
        {
            Logging.Log(TraceEventType.Information, Manifest.Forsendelse.KonversasjonsId, string.Format("Krypterer dokumentpakke med sertifikat {0}.", _krypteringssertifikat.Thumbprint));

            var contentInfo = new ContentInfo(bytes);
            var encryptAlgoOid = new Oid("2.16.840.1.101.3.4.1.42"); // AES-256-CBC            
            var envelopedCms = new EnvelopedCms(contentInfo, new AlgorithmIdentifier(encryptAlgoOid));
            var recipient = new CmsRecipient(_krypteringssertifikat);
            envelopedCms.Encrypt(recipient);
            return envelopedCms.Encode();
        }

        public static byte[] Dekrypter(byte[] kryptertData)
        {
            var envelopedCms = new EnvelopedCms();
            envelopedCms.Decode(kryptertData);
            envelopedCms.Decrypt(envelopedCms.RecipientInfos[0]);
            return envelopedCms.ContentInfo.Content;
        }
    }
}
