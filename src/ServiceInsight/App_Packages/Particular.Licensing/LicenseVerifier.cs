namespace Particular.Licensing
{
    using System;
    using System.Security.Cryptography;
    using System.Security.Cryptography.Xml;
    using System.Xml;

    class LicenseVerifier
    {
        public static bool TryVerify(string licenseText, out string failureMessage)
        {
            try
            {
                if (string.IsNullOrEmpty(licenseText))
                {
                    failureMessage = "Empty license string";
                    return false;
                }

                return SignedXmlVerifier.TryVerifyXml(licenseText, out failureMessage);
            }
            catch (Exception ex)
            {
                failureMessage = ex.Message;
                return false;
            }
        }

        const string Modulus = "5M9/p7N+JczIN/e5eObahxeCIe//2xRLA9YTam7zBrcUGt1UlnXqL0l/8uO8rsO5tl+tjjIV9bOTpDLfx0H03VJyxsE8BEpSVu48xujvI25+0mWRnk4V50bDZykCTS3Du0c8XvYj5jIKOHPtU//mKXVULhagT8GkAnNnMj9CvTc=";
        const string Exponent = "AQAB";

        public const string PublicKey = "<RSAKeyValue><Modulus>" + Modulus + "</Modulus><Exponent>" + Exponent + "</Exponent></RSAKeyValue>";

        static class SignedXmlVerifier
        {
            public static bool TryVerifyXml(string xml, out string failureMessage)
            {
                if (!TryLoadXmlDocument(xml, out var doc))
                {
                    failureMessage = "The text provided could not be parsed as XML";
                    return false;
                }

                using (var rsa = new RSACryptoServiceProvider())
                {
                    var parameters = new RSAParameters
                    {
                        Modulus = Convert.FromBase64String(Modulus),
                        Exponent = Convert.FromBase64String(Exponent)
                    };

                    rsa.ImportParameters(parameters);

                    var nsMgr = new XmlNamespaceManager(doc.NameTable);
                    nsMgr.AddNamespace("sig", "http://www.w3.org/2000/09/xmldsig#");

                    var signedXml = new SignedXml(doc);
                    var signature = (XmlElement)doc.SelectSingleNode("//sig:Signature", nsMgr);

                    if (signature == null)
                    {
                        failureMessage = "XML is invalid because does not have an XML signature";
                        return false;
                    }

                    signedXml.LoadXml(signature);

                    if (!signedXml.CheckSignature(rsa))
                    {
                        failureMessage = "XML is invalid because it failed the signature check";
                        return false;
                    }

                    failureMessage = null;
                    return true;
                }
            }

            static bool TryLoadXmlDocument(string xml, out XmlDocument xmlDocument)
            {
                try
                {
                    var doc = new XmlDocument();
                    doc.LoadXml(xml);

                    xmlDocument = doc;
                    return true;
                }
                catch (XmlException)
                {
                    xmlDocument = null;
                    return false;
                }
            }
        }
    }
}