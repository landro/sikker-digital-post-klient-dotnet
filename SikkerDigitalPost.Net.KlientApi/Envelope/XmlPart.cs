﻿using System.Xml;
using SikkerDigitalPost.Net.Domene.Entiteter;
using SikkerDigitalPost.Net.Domene.Entiteter.Aktører;
using SikkerDigitalPost.Net.Domene.Entiteter.Post;

namespace SikkerDigitalPost.Net.KlientApi.Envelope
{
    public abstract class XmlPart
    {
        protected readonly XmlDocument XmlEnvelope;
        protected readonly Forsendelse Forsendelse;
        protected readonly AsicEArkiv AsicEArkiv;
        protected readonly Databehandler Databehandler;

        protected XmlPart(XmlDocument xmlEnvelope, Forsendelse forsendelse, AsicEArkiv asicEArkiv, Databehandler databehandler)
        {
            XmlEnvelope = xmlEnvelope;
            Forsendelse = forsendelse;
            AsicEArkiv = asicEArkiv;
            Databehandler = databehandler;
        }

        public abstract XmlElement Xml();

    }
}