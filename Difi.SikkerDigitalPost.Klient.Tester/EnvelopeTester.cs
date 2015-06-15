/** 
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

using System.Security;
using System.Security.Cryptography;
using System.Xml;
using Difi.SikkerDigitalPost.Klient.Security;
using Difi.SikkerDigitalPost.Klient.XmlValidering;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Difi.SikkerDigitalPost.Klient.Tester
{
    [TestClass]
    public class EnvelopeTester : TestBase
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Initialiser();
        }

        [TestMethod]
        [SecuritySafeCritical]
        public void ValidereEnvelopeMotXsdValiderer()
        {
            const string signatureMethod = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";

            if (CryptoConfig.CreateFromName(signatureMethod) == null)
                CryptoConfig.AddAlgorithm(typeof(RsaPkCs1Sha256SignatureDescription), signatureMethod);

            var forretningsmeldingEnvelopeXml = Envelope.Xml();
            var  envelopeValidator = new ForretningsmeldingEnvelopeValidator();
            var validert = envelopeValidator.ValiderDokumentMotXsd(forretningsmeldingEnvelopeXml.OuterXml);

            Assert.IsTrue(validert, envelopeValidator.ValideringsVarsler);
        }

        [TestMethod]
        [SecuritySafeCritical]
        public void LagUgyldigSecurityNodeXsdValidererIkke()
        {
            const string signatureMethod = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";

            if (CryptoConfig.CreateFromName(signatureMethod) == null)
                CryptoConfig.AddAlgorithm(typeof(RsaPkCs1Sha256SignatureDescription), signatureMethod);

           var forretningsmeldingEnvelopeXml = Envelope.Xml();
            var envelopeValidator = new ForretningsmeldingEnvelopeValidator();

            //Endre til ugyldig forretningsmeldingenvelope
            var namespaceManager = new XmlNamespaceManager(forretningsmeldingEnvelopeXml.NameTable);
            namespaceManager.AddNamespace("env", Navnerom.SoapEnvelopeEnv12);
            namespaceManager.AddNamespace("eb", Navnerom.EbXmlCore);
            namespaceManager.AddNamespace("ds", Navnerom.XmlDsig);
            namespaceManager.AddNamespace("wsse", Navnerom.WssecuritySecext10);
            namespaceManager.AddNamespace("wsu", Navnerom.WssecurityUtility10);
            namespaceManager.AddNamespace("ns3", Navnerom.StandardBusinessDocumentHeader);
            namespaceManager.AddNamespace("ns9", Navnerom.DifiSdpSchema10);
            namespaceManager.AddNamespace("ns5", Navnerom.XmlDsig);

            var securityNode = forretningsmeldingEnvelopeXml.DocumentElement.SelectSingleNode("//wsse:Security", namespaceManager);

            var gammelVerdi = securityNode.Attributes["mustUnderstand"].Value;
            securityNode.Attributes["mustUnderstand"].Value = "en_tekst_som_ikke_er_bool";

            var validert = envelopeValidator.ValiderDokumentMotXsd(forretningsmeldingEnvelopeXml.OuterXml);
            Assert.IsFalse(validert, envelopeValidator.ValideringsVarsler);

            securityNode.Attributes["mustUnderstand"].Value = gammelVerdi;
        }
    }
}
