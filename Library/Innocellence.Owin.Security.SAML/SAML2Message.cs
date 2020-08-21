// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens;
using System.Net.Http;
using Microsoft.IdentityModel.Extensions;
using Microsoft.IdentityModel.Protocols;
using System.Text;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Xml;
using System.IdentityModel.Protocols.WSTrust;
using System.Security.Cryptography.X509Certificates;
using System.IdentityModel.Selectors;
using System.Collections.ObjectModel;
using System.IdentityModel.Metadata;
using System.Security.Cryptography;

namespace DLYB.Owin.Security.SAML
{
     [SuppressMessage("Microsoft.Naming", "CA1704")]
    public class SAML2Message 
    {

         private Dictionary<string, string> _parameters = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new <see cref="SAMLAuthenticationOptions"/>
        /// </summary>
        public SAML2Message()
        {
        }
        public SAML2Message(IEnumerable<KeyValuePair<string, string[]>> parameters)
        {
            if (parameters == null)
            {
                return;
            }
            foreach (KeyValuePair<string, string[]> keyValue in parameters)
            {
                string[] value = keyValue.Value;
                for (int i = 0; i < value.Length; i++)
                {
                    string strValue = value[i];
                    this.SetParameter(keyValue.Key, strValue);
                }
            }
        }



        public string SAMLResponse
        {
            get
            {
                return this.GetParameter("SAMLResponse");
            }
            set
            {
                this.SetParameter("SAMLResponse", value);
            }
        }
         

        public string IssuerAddress
        {
            get;
            set;
        }

        public string state
        {
            get;
            set;
        }

        static internal readonly string RsaSha256Namespace = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";

    

        /// <summary>
        /// Url to discovery service to use if no idp is specified in the sign in call.
        /// </summary>

        /// <summary>
        /// EntityId - The identity of the ServiceProvider to use when sending requests to Idp
        /// and presenting the SP in metadata.
        /// </summary>
       

        public string Wresult
        {
            get
            {
                return this.GetParameter("SAMLResponse");
            }
            set
            {
                this.SetParameter("SAMLResponse", value);
            }
        }

        public string TargetResource
        {
            get
            {
                return this.GetParameter("TargetResource");
            }
            set
            {
                this.SetParameter("TargetResource", value);
            }
        }

        public string PartnerSpId
        {
            get
            {
                return this.GetParameter("PartnerSpId");
            }
            set
            {
                this.SetParameter("PartnerSpId", value);
            }
        }
        public string IdpAdapterId
        {
            get
            {
                return this.GetParameter("IdpAdapterId");
            }
            set
            {
                this.SetParameter("IdpAdapterId", value);
            }
        }
        public string RelayState
        {
            get
            {
                return this.GetParameter("RelayState");
            }
            set
            {
                this.SetParameter("RelayState", value);
            }
        }

         

        public virtual string GetParameter(string parameter)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                throw new ArgumentNullException("parameter");
            }
            string value = null;
            this._parameters.TryGetValue(parameter, out value);
            return value;
        }

        public void SetParameter(string parameter, string value)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                throw new ArgumentNullException("parameter");
            }
            if (value == null)
            {
                if (this._parameters.ContainsKey(parameter))
                {
                    this._parameters.Remove(parameter);
                    return;
                }
            }
            else
            {
                this._parameters[parameter] = value;
            }
        }

        public string CreateSignOutUrl()
        {
            return "SignOut";
        }

        

      //  public IEnumerable<X509Certificate> SigningKeys { get; set; }


         

        public string CertFilePath { get; set; }

        public string CreateSignInUrl()
        {
            StringBuilder strBuilder = new StringBuilder(this.IssuerAddress);
            bool issuerAddressHasQuery = this.IssuerAddress.Contains("?");
            foreach (KeyValuePair<string, string> parameter in this._parameters)
            {
                if (!string.IsNullOrEmpty( parameter.Value) )
                {
                    if (!issuerAddressHasQuery)
                    {
                        strBuilder.Append('?');
                        issuerAddressHasQuery = true;
                    }
                    else
                    {
                        strBuilder.Append('&');
                    }
                    strBuilder.Append(HttpUtility.UrlEncode(parameter.Key));
                    strBuilder.Append('=');
                    strBuilder.Append(HttpUtility.UrlEncode(parameter.Value));
                }
            }
            return strBuilder.ToString();
        }

        public virtual Saml2Response GetToken()
        {
            //if (this.Wresult == null)
            //{
            //    return null;
            //}
            //using (StringReader sr = new StringReader(this.Wresult))
            //{
            //    XmlReader xmlReader = XmlReader.Create(sr);
            //    xmlReader.MoveToContent();
            //    WSTrustResponseSerializer serializer = new WSTrust13ResponseSerializer();
            //    if (serializer.CanRead(xmlReader))
            //    {
            //        RequestSecurityTokenResponse response = serializer.ReadXml(xmlReader, new WSTrustSerializationContext());
            //        string outerXml = response.RequestedSecurityToken.SecurityTokenXml.OuterXml;
            //        return outerXml;
            //    }
            //    serializer = new WSTrustFeb2005ResponseSerializer();
            //    if (serializer.CanRead(xmlReader))
            //    {
            //        RequestSecurityTokenResponse response2 = serializer.ReadXml(xmlReader, new WSTrustSerializationContext());
            //        string outerXml = response2.RequestedSecurityToken.SecurityTokenXml.OuterXml;
            //        return outerXml;
            //    }
            //}



          ////  string certificatePath = @"D:\Projects\SAMLDemo\Server.pfx";
          //  X509Certificate2 cert = ServiceCertificate;// new X509Certificate2(CertFilePath);

          //  //TextReader

          //  StringReader strRdr = new StringReader(Encoding.Default.GetString(Convert.FromBase64String(SAMLResponse)));

          // // string samlFilePath = @"D:\Projects\SAMLDemo\saml.xml";
          // XmlReader reader = XmlReader.Create(strRdr);

          //  List<SecurityToken> tokens = new List<SecurityToken>();
          //  tokens.Add(new X509SecurityToken(cert));

          //  SecurityTokenResolver outOfBandTokenResolver = SecurityTokenResolver.CreateDefaultSecurityTokenResolver(new ReadOnlyCollection<SecurityToken>(tokens), true);
          //  SecurityToken securityToken = System.ServiceModel.Security.WSSecurityTokenSerializer.DefaultInstance.ReadToken(reader, outOfBandTokenResolver);

          //  SamlSecurityToken deserializedSaml = securityToken as SamlSecurityToken;



            var samlResponse = Saml2Response.Read(Encoding.Default.GetString(Convert.FromBase64String(SAMLResponse)));




            return samlResponse;
        }

    }
}