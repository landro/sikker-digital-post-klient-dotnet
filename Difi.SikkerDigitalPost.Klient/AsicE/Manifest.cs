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

using System;
using System.Diagnostics;
using System.Text;
using System.Xml;
using Difi.SikkerDigitalPost.Klient.Domene.Entiteter.Aktører;
using Difi.SikkerDigitalPost.Klient.Domene.Entiteter.Interface;
using Difi.SikkerDigitalPost.Klient.Domene.Entiteter.Post;
using Difi.SikkerDigitalPost.Klient.Domene.Enums;
using Difi.SikkerDigitalPost.Klient.Domene.Extensions;

namespace Difi.SikkerDigitalPost.Klient.AsicE
{
    internal class Manifest : IAsiceVedlegg
    {
        private XmlDocument _manifestXml;

        public Avsender Avsender { get; private set; }
        public Forsendelse Forsendelse { get; private set; }


        public Manifest(Forsendelse forsendelse)
        {
            Forsendelse = forsendelse;
            Avsender = forsendelse.Avsender;
        }

        public string Filnavn
        {
            get { return "manifest.xml"; }
        }

        public string MimeType
        {
            get { return "application/xml"; }
        }

        public string Id
        {
            get
            {
                return "Id_1";
            }
        }

        public byte[] Bytes
        {
            get
            {
                return Encoding.UTF8.GetBytes(Xml().OuterXml);
            }
        }

        public XmlDocument Xml()
        {
            if (_manifestXml != null)
                return _manifestXml;

            _manifestXml = new XmlDocument { PreserveWhitespace = true };
            var xmlDeclaration = _manifestXml.CreateXmlDeclaration("1.0", "UTF-8", null);
            _manifestXml.AppendChild(_manifestXml.CreateElement("manifest", Navnerom.DifiSdpSchema10));
            _manifestXml.InsertBefore(xmlDeclaration, _manifestXml.DocumentElement);

            if (Forsendelse.Sendes(Postmetode.Digital))
            {
                _manifestXml.DocumentElement.AppendChild(MottakerNode());
            }

            _manifestXml.DocumentElement.AppendChild(AvsenderNode());

            var hoveddokument = Forsendelse.Dokumentpakke.Hoveddokument;
            _manifestXml.DocumentElement.AppendChild(DokumentNode(hoveddokument, "hoveddokument", hoveddokument.Tittel));

            foreach (var vedlegg in Forsendelse.Dokumentpakke.Vedlegg)
            {
                _manifestXml.DocumentElement.AppendChild(DokumentNode(vedlegg, "vedlegg", vedlegg.Filnavn));
            }

            Logging.Log(TraceEventType.Verbose, Forsendelse.KonversasjonsId, "Generert manifest for dokumentpakke" + Environment.NewLine + _manifestXml.OuterXml);

            return _manifestXml;
        }

        private XmlElement MottakerNode()
        {
           var digitalMottaker = (DigitalPostMottaker) Forsendelse.PostInfo.Mottaker;

            var mottaker = _manifestXml.CreateElement("mottaker", Navnerom.DifiSdpSchema10);

            XmlElement person = _manifestXml.CreateElement("person", Navnerom.DifiSdpSchema10);
            {
                XmlElement personidentifikator = person.AppendChildElement("personidentifikator", Navnerom.DifiSdpSchema10, _manifestXml);
                personidentifikator.InnerText = digitalMottaker.Personidentifikator;

                XmlElement postkasseadresse = person.AppendChildElement("postkasseadresse", Navnerom.DifiSdpSchema10, _manifestXml);
                postkasseadresse.InnerText = digitalMottaker.Postkasseadresse;
            }

            mottaker.AppendChild(person);
            return mottaker;
        }

        private XmlElement AvsenderNode()
        {
            XmlElement avsender = _manifestXml.CreateElement("avsender", Navnerom.DifiSdpSchema10);
            {
                XmlElement organisasjon = avsender.AppendChildElement("organisasjon", Navnerom.DifiSdpSchema10, _manifestXml);
                organisasjon.SetAttribute("authority", "iso6523-actorid-upis");
                organisasjon.InnerText = Avsender.Organisasjonsnummer.Iso6523();

                var avsenderId = Avsender.Avsenderidentifikator;
                if (!String.IsNullOrWhiteSpace(avsenderId))
                {
                    XmlElement avsenderidentifikator = avsender.AppendChildElement("avsenderidentifikator", Navnerom.DifiSdpSchema10, _manifestXml);
                    avsenderidentifikator.InnerText = Avsender.Avsenderidentifikator;
                }

                XmlElement fakturaReferanse = avsender.AppendChildElement("fakturaReferanse", Navnerom.DifiSdpSchema10, _manifestXml);
                fakturaReferanse.InnerText = Avsender.Fakturareferanse;
            }

            return avsender;
        }

        private XmlElement DokumentNode(Dokument dokument, string elementnavn, string innholdstekst)
        {
            XmlElement dokumentXml = _manifestXml.CreateElement(elementnavn, Navnerom.DifiSdpSchema10);
            dokumentXml.SetAttribute("href", dokument.VasketFilnavn);
            dokumentXml.SetAttribute("mime", dokument.MimeType);
            {
                XmlElement tittel = dokumentXml.AppendChildElement("tittel", Navnerom.DifiSdpSchema10, _manifestXml);
                tittel.SetAttribute("lang", dokument.Språkkode ?? Forsendelse.Språkkode);
                tittel.InnerText = innholdstekst;
            }
            return dokumentXml;
        }

    }
}
