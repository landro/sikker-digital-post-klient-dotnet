﻿using System;
using System.IO;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace SikkerDigitalPost.Klient.Security
{
    /// <summary>
    /// Represents the attachment content signature transform as defined in the Web Services SecurityElement SOAP Message with Attachments (SwA) Profile for use with binary attachments. 
    /// </summary>
    internal class AttachmentContentSignatureTransform : Transform
    {
        private readonly Type[] _inputTypes = new Type[2] {
            typeof(Stream),
            typeof(byte[])
        };

        private readonly Type[] _outputTypes = new Type[1] {
            typeof (Stream)
        };

        private Stream _result;

        /// <summary>
        /// Initializes a new instance of the AttachmentContentSignatureTransform class.
        /// </summary>
        public AttachmentContentSignatureTransform()
        {
            Algorithm = "http://docs.oasis-open.org/wss/oasis-wss-SwAProfile-1.1#Attachment-Content-Signature-Transform";
        }

        protected override XmlNodeList GetInnerXml()
        {
            return null;
        }

        public override object GetOutput(Type type)
        {
            return GetOutput();
        }

        public override object GetOutput()
        {
            return _result;
        }
        
        /// <summary>
        /// Returns a list of the valid input types for this transformer. Valid types are Stream and byte[].
        /// </summary>
        public override Type[] InputTypes
        {
            get { return _inputTypes; }
        }

        public override void LoadInnerXml(XmlNodeList nodeList)
        {
        }

        /// <summary>
        /// Loads the data for transformation. Valid types are Stream or byte[].
        /// </summary>
        /// <param name="obj">The data for transformation.</param>
        public override void LoadInput(object obj)
        {
            if (obj is Stream)
            {
                _result = new MemoryStream();
                ((Stream)obj).CopyTo(_result);
                if (((Stream)obj).CanSeek)
                    ((Stream)obj).Position = 0;

                _result.Position = 0;
            }
            else
            {
                _result = new MemoryStream((byte[])obj);
            }
        }

        public override Type[] OutputTypes
        {
            get { return _outputTypes; }
        }
    }
}