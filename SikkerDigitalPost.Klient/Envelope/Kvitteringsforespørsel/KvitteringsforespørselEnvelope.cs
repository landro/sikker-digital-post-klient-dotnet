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

using System.Xml;
using SikkerDigitalPost.Klient.Envelope.Abstract;

namespace SikkerDigitalPost.Klient.Envelope.Kvitteringsforespørsel
{
    internal class KvitteringsforespørselEnvelope : AbstractEnvelope
    {
        public KvitteringsforespørselEnvelope(EnvelopeSettings settings) : base(settings)
        {
        }

        protected override XmlNode HeaderElement()
        {
            Header = new KvitteringsforespørselHeader(Settings, EnvelopeXml);
            return Header.Xml();
        }

        protected override XmlNode BodyElement()
        {
            var body = new UtenInnholdBody(Settings, EnvelopeXml);
            return body.Xml();
        }
    }
}
