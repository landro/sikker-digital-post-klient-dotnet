﻿using System;
using System.Xml;
using SikkerDigitalPost.Net.Domene.Entiteter;
using SikkerDigitalPost.Net.KlientApi.Envelope.EnvelopeBody;

namespace SikkerDigitalPost.Net.KlientApi.Envelope.Body
{
    public class StandardBusinessDocument : XmlPart
    {
        private readonly DateTime _creationDateAndtime;
        
        public StandardBusinessDocument(XmlDocument dokument, Forsendelse forsendelse, Arkiv arkiv, Databehandler databehandler) : base(dokument,forsendelse, arkiv, databehandler)
        {
            _creationDateAndtime = DateTime.UtcNow;
        }

        public override XmlElement Xml()
        {
            var sbdElement = XmlDocument.CreateElement("ns3", "StandardBusinessDocument", Navnerom.Ns3);
            sbdElement.SetAttribute("xmlns:ns3", Navnerom.Ns3);
            sbdElement.SetAttribute("xmlns:ns5", Navnerom.Ns5);
            sbdElement.SetAttribute("xmlns:ns9", Navnerom.Ns9);

            var sbdHeader = new StandardBusinessDocumentHeader(XmlDocument, Forsendelse, Arkiv, Databehandler, _creationDateAndtime);
            sbdElement.AppendChild(sbdHeader.Xml());

            var digitalPost = new DigitalPostElement(XmlDocument, Forsendelse, Arkiv, Databehandler);

            sbdElement.AppendChild(digitalPost.Xml());

            return sbdElement;
        }
    }
}
