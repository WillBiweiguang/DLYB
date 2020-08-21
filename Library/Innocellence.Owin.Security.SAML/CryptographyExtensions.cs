using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml;

namespace DLYB.Owin.Security.SAML
{
    internal static class CryptographyExtensions
    {
        internal static IEnumerable<XmlElement> Decrypt(this IEnumerable<XmlElement> elements, AsymmetricAlgorithm key)
        {
            foreach (var element in elements)
            {
                var xmlDoc = new XmlDocument { PreserveWhitespace = true };
                xmlDoc.LoadXml(element.OuterXml);

                var exml = new RSAEncryptedXml(xmlDoc, (RSA)key);

                exml.DecryptDocument();

                yield return xmlDoc.DocumentElement;
            }
        } 
    }
}
