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
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using Difi.SikkerDigitalPost.Klient.Domene.Entiteter.Interface;
using Difi.SikkerDigitalPost.Klient.Domene.Extensions;
using Difi.SikkerDigitalPost.Klient.Utilities;

namespace Difi.SikkerDigitalPost.Klient.Security
{
    internal class QualifyingPropertiesObject : DataObject
    {
        public X509Certificate2 Certificate { get; private set; }

        public IAsiceVedlegg[] References { get; private set; }

        /// <summary>
        /// The mandatory Target attribute refers to the XML signature in which the qualifying properties are associated.
        /// </summary>
        public string Target { get; private set; }

        /// <param name="certificate">The certificate used for signing.</param>
        /// <param name="target">The target attribute value of the QualifyingProperties element.</param>
        /// <param name="references">List of DataObjectFormat elements to be included.</param>
        /// <param name="context">The element where the signature will be placed. Used for extracting namespaces.</param>
        public QualifyingPropertiesObject(X509Certificate2 certificate, string target, IAsiceVedlegg[] references, XmlElement context)
        {
            Certificate = certificate;
            Target = target;
            References = references;
            Data = CreateNodes(context);
        }

        private XmlNodeList CreateNodes(XmlElement context)
        {
            // To get the digest value for the qualifying properties node, the namespaces of the containing document needs to be available. This is problematic, as the signature element is not added
            // to the document until after the signature has been calculated. To circumvent this, we take the position where the signature will be added (XmlElement context parameter), clone its document
            // and add the QualifyingProperties inside a 'dummy' signature element. This ensures that the canoncalization process will process the QualifyingProperties parent nodes for namespaces.


            // Clone of the target document for the signature.
            var clone = (XmlDocument)context.OwnerDocument.Clone();
            clone.PreserveWhitespace = true;

            // Find where the signature is to be inserted in the cloned document. In our scenario, the signature is placed as a child of the root XAdESSignatures element.
            var cloneContext = (XmlElement)clone.DocumentElement;

            // Create a 'dummy' signature node where the QualifyingProperties will be positioned.
            var signature = cloneContext.AppendChild("Signature", Navnerom.XmlDsig);

            // Add the QualifyingProperties node as normal. This node will be set as the Objects Data property.
            var root = signature.AppendChild("QualifyingProperties", Navnerom.UriEtsi132);
            root.SetAttribute("Target", this.Target);

            // Create Xml Node List
            var signedProperties = root.AppendChild("SignedProperties", Navnerom.UriEtsi132);

            signedProperties.SetAttribute("Id", "SignedProperties");

            var signedSignatureProperties = signedProperties.AppendChild("SignedSignatureProperties", Navnerom.UriEtsi132);
            signedSignatureProperties.AppendChild("SigningTime", Navnerom.UriEtsi132, DateTime.UtcNow.ToString(DateUtility.DateFormat, CultureInfo.InvariantCulture));
            var signingCertificate = signedSignatureProperties.AppendChild("SigningCertificate", Navnerom.UriEtsi132);

            var cert = signingCertificate.AppendChild("Cert", Navnerom.UriEtsi132);

            var certDigest = cert.AppendChild("CertDigest", Navnerom.UriEtsi132);
            certDigest.AppendChild("DigestMethod", Navnerom.XmlDsig).SetAttribute("Algorithm", "http://www.w3.org/2000/09/xmldsig#sha1");
            certDigest.AppendChild("DigestValue", Navnerom.XmlDsig, Convert.ToBase64String(Certificate.GetCertHash()));

            var issuerSerial = cert.AppendChild("IssuerSerial", Navnerom.UriEtsi132);
            issuerSerial.AppendChild("X509IssuerName", Navnerom.XmlDsig, Certificate.Issuer);
            //issuerSerial.AppendChild("X509SerialNumber", Navnerom.XmlDsig, BigInteger.Parse(Certificate.SerialNumber, NumberStyles.HexNumber).ToString());

            var signedDataObjectProperties = signedProperties.AppendChild("SignedDataObjectProperties", Navnerom.UriEtsi132);
            foreach (var item in References)
            {
                var a = signedDataObjectProperties.AppendChild("DataObjectFormat", Navnerom.UriEtsi132);
                a.SetAttribute("ObjectReference", "#" + item.Id);
                a.AppendChild("MimeType", "http://uri.etsi.org/01903/v1.3.2#", item.MimeType);
            }

            return root.SelectNodes(".");
        }
    }
}
