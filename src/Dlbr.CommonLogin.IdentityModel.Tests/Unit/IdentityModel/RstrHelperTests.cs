﻿using System;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using Dlbr.CommonLogin.IdentityModel.Windows;
using NUnit.Framework;
using XmlSpecificationCompare.NUnit;

namespace Dlbr.CommonLogin.IdentityModel.Tests.Unit.IdentityModel
{
    [TestFixture]
    [Category("BuildVerification")]
    public class RstrHelperTests
    {
        const string WSTrustvOldRstr = @"<t:RequestSecurityTokenResponse xmlns:t=""http://schemas.xmlsoap.org/ws/2005/02/trust""><t:Lifetime><wsu:Created xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">2016-02-22T12:22:00.047Z</wsu:Created><wsu:Expires xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">2016-02-22T13:22:00.047Z</wsu:Expires></t:Lifetime><wsp:AppliesTo xmlns:wsp=""http://schemas.xmlsoap.org/ws/2004/09/policy""><wsa:EndpointReference xmlns:wsa=""http://www.w3.org/2005/08/addressing""><wsa:Address>https://dev-www-dlbrlogin.vfltest.dk/WebTestApp/</wsa:Address></wsa:EndpointReference></wsp:AppliesTo><t:RequestedSecurityToken><saml:Assertion MajorVersion=""1"" MinorVersion=""1"" AssertionID=""_85714861-963d-4d9d-a2b8-7f80f7386dbc"" Issuer=""http://dev-idp.vfltest.dk/adfs/services/trust"" IssueInstant=""2016-02-22T12:22:00.062Z"" xmlns:saml=""urn:oasis:names:tc:SAML:1.0:assertion""><saml:Conditions NotBefore=""2016-02-22T12:22:00.047Z"" NotOnOrAfter=""2016-02-22T13:22:00.047Z""><saml:AudienceRestrictionCondition><saml:Audience>https://dev-www-dlbrlogin.vfltest.dk/WebTestApp/</saml:Audience></saml:AudienceRestrictionCondition></saml:Conditions><saml:AttributeStatement><saml:Subject><saml:NameIdentifier>mac@PROD.DLI</saml:NameIdentifier><saml:SubjectConfirmation><saml:ConfirmationMethod>urn:oasis:names:tc:SAML:1.0:cm:bearer</saml:ConfirmationMethod></saml:SubjectConfirmation></saml:Subject><saml:Attribute AttributeName=""name"" AttributeNamespace=""http://schemas.xmlsoap.org/ws/2005/05/identity/claims""><saml:AttributeValue>Michael Christensen (PROD\mac)</saml:AttributeValue></saml:Attribute></saml:AttributeStatement><saml:AuthenticationStatement AuthenticationMethod=""urn:oasis:names:tc:SAML:1.0:am:password"" AuthenticationInstant=""2016-02-22T12:21:58.047Z""><saml:Subject><saml:NameIdentifier>mac@PROD.DLI</saml:NameIdentifier><saml:SubjectConfirmation><saml:ConfirmationMethod>urn:oasis:names:tc:SAML:1.0:cm:bearer</saml:ConfirmationMethod></saml:SubjectConfirmation></saml:Subject></saml:AuthenticationStatement><ds:Signature xmlns:ds=""http://www.w3.org/2000/09/xmldsig#""><ds:SignedInfo><ds:CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></ds:CanonicalizationMethod><ds:SignatureMethod Algorithm=""http://www.w3.org/2001/04/xmldsig-more#rsa-sha256""></ds:SignatureMethod><ds:Reference URI=""#_85714861-963d-4d9d-a2b8-7f80f7386dbc""><ds:Transforms><ds:Transform Algorithm=""http://www.w3.org/2000/09/xmldsig#enveloped-signature""></ds:Transform><ds:Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></ds:Transform></ds:Transforms><ds:DigestMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#sha256""></ds:DigestMethod><ds:DigestValue>mfdPrlsKTQhdGz0CzsjAaGoeOxQk029ZPCJ+ox4aJT8=</ds:DigestValue></ds:Reference></ds:SignedInfo><ds:SignatureValue>tPHSr1wkFCV8r7pPpEjaq24FbazML4WDQpS5i+6wX7Op7VhqoxE34OydP/CbzYJhJOOhY9KSJCd30z8+xZHXSHYBX7JBmGeEZr8DNglLZOaK28I0ndePTNvwkWo7Dr/ltuH32ysa255NKKdEs7E8rNMUAe+aMdD5JmacgUZ5JbovW/hYhL2UwjX0JVT13dt/shJeItmubvqdvxquLES3lg1D5u9EkKFoJeroJBxCmMDZPrZqm2neXDI8rqWUV2TFcxrgIQo5B+NyjJhdEeSL3oBRygfixE+YL92aiRnO9g4OpmBkqpmkhc5/6UrxqwyqddOsADjE0XsV4RDSibsz4g==</ds:SignatureValue><KeyInfo xmlns=""http://www.w3.org/2000/09/xmldsig#""><X509Data><X509Certificate>MIIDOjCCAiagAwIBAgIQTVNRSnGxEK1AHSMjgTpZRjAJBgUrDgMCHQUAMCQxIjAgBgNVBAMTGWRldi1hZGZzc2lnbmluZy52ZmwubG9jYWwwIBcNMTIxMDAyMDczNzQzWhgPMjk5ODEyMzEyMjAwMDBaMCQxIjAgBgNVBAMTGWRldi1hZGZzc2lnbmluZy52ZmwubG9jYWwwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQC2q8hgBQ6v0yVElSlCxkCyfEPtPyeZyaNNUeZBMzW7IK2K0KsoXb9tJU0yX0tRZsPbtXSvwnhLDbeqkv9PlxlQWYt1KcJz6F7Zw49Jd0h07VCzweG4sqFWqh8m1CF7lGULveNyBWQ1gtI9akWvgLKoMpHicGlHpBslE7mghB2EhvJfvd7ZanKUINr+PPNxx83CJa6j4tS/x5At0tRcgj0YMjQWYoUke4+vYKmAEVgN7a0JvSR7XiDCTA1PIdIrmk6MS8Rw3JDFIPeeM24nE5IBwUsZOR8kUWuDULdXVezX7nClgygF//3pD2hcZ2ZB0rg3X9cW8gyfdTnVdqH8PUdPAgMBAAGjbjBsMBMGA1UdJQQMMAoGCCsGAQUFBwMBMFUGA1UdAQROMEyAEDE5uBZioFVgHByQguJ46vqhJjAkMSIwIAYDVQQDExlkZXYtYWRmc3NpZ25pbmcudmZsLmxvY2FsghBNU1FKcbEQrUAdIyOBOllGMAkGBSsOAwIdBQADggEBAB1+/OQG36b2QiYjuoHR+AVPdMFZ/Tv2i629C0QPdbcgWw2zL9OGzMQZW6k7Kzsh9NXELWjcMo3vpJlT4S/G4qFggQ0jhkiXSqdCOi4LOS5HLzWbDY6S1Xl64NPqVxPiYum2/wOsA9/3KXqCkW31h4271n1gc+u2jM56axXxuIiL2/ILS49TGOd2+pHMcexb3o/Wn/bR8rA/0/nrdoTH8bBR+fSEgWBvn2YfqtbcBWkD95v/gjG6s7uqp9g+bBUfj4F6/R0OL+d/B/Yww6cwtd1GYT/eIB9qQVcsZgD7QvHJQ6eZS2pgvpo5OaPJirSHcJ68nA3VAvhS6zc1JGSRv8g=</X509Certificate></X509Data></KeyInfo></ds:Signature></saml:Assertion></t:RequestedSecurityToken><t:TokenType>urn:oasis:names:tc:SAML:1.0:assertion</t:TokenType><t:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</t:RequestType><t:KeyType>http://schemas.xmlsoap.org/ws/2005/05/identity/NoProofKey</t:KeyType></t:RequestSecurityTokenResponse>";
        const string WSTrust13Rstr = @"<trust:RequestSecurityTokenResponseCollection xmlns:trust=""http://docs.oasis-open.org/ws-sx/ws-trust/200512""><trust:RequestSecurityTokenResponse><trust:Lifetime><wsu:Created xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">2016-02-22T12:17:27.241Z</wsu:Created><wsu:Expires xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">2016-02-22T13:17:27.241Z</wsu:Expires></trust:Lifetime><wsp:AppliesTo xmlns:wsp=""http://schemas.xmlsoap.org/ws/2004/09/policy""><wsa:EndpointReference xmlns:wsa=""http://www.w3.org/2005/08/addressing""><wsa:Address>https://localhost.vfltest.dk/OBAPI/</wsa:Address></wsa:EndpointReference></wsp:AppliesTo><trust:RequestedSecurityToken><saml:Assertion MajorVersion=""1"" MinorVersion=""1"" AssertionID=""_4c4b13c1-163e-4654-b9ce-38d4898c8140"" Issuer=""http://devtest-idp.vfltest.dk/adfs/services/trust"" IssueInstant=""2016-02-22T12:17:27.257Z"" xmlns:saml=""urn:oasis:names:tc:SAML:1.0:assertion""><saml:Conditions NotBefore=""2016-02-22T12:17:27.241Z"" NotOnOrAfter=""2016-02-22T13:17:27.241Z""><saml:AudienceRestrictionCondition><saml:Audience>https://localhost.vfltest.dk/OBAPI/</saml:Audience></saml:AudienceRestrictionCondition></saml:Conditions><saml:AttributeStatement><saml:Subject><saml:NameIdentifier>cvruser1@PROD.DLI</saml:NameIdentifier><saml:SubjectConfirmation><saml:ConfirmationMethod>urn:oasis:names:tc:SAML:1.0:cm:bearer</saml:ConfirmationMethod></saml:SubjectConfirmation></saml:Subject><saml:Attribute AttributeName=""role"" AttributeNamespace=""http://schemas.microsoft.com/ws/2008/06/identity/claims""><saml:AttributeValue>GBTC009Landmand</saml:AttributeValue><saml:AttributeValue>GBTOLandmand</saml:AttributeValue><saml:AttributeValue>GTALCLandmandDKSeClaims</saml:AttributeValue><saml:AttributeValue>Domain Users</saml:AttributeValue><saml:AttributeValue>GTALCCSSystembrugere</saml:AttributeValue><saml:AttributeValue>GTALCDLIBasis</saml:AttributeValue><saml:AttributeValue>GTALCPlanteITBasis</saml:AttributeValue><saml:AttributeValue>GBTC006Landmand</saml:AttributeValue><saml:AttributeValue>GTAFocusFinderAdgang</saml:AttributeValue><saml:AttributeValue>GTALCOBAPITest</saml:AttributeValue><saml:AttributeValue>GCAktivemedlemmer</saml:AttributeValue><saml:AttributeValue>GC006Aktivemedlemmer</saml:AttributeValue><saml:AttributeValue>GAALCMidtuge001</saml:AttributeValue><saml:AttributeValue>GBTDLLandmand</saml:AttributeValue><saml:AttributeValue>GTALCLandmandDkMedlem</saml:AttributeValue><saml:AttributeValue>GTALCDCFFodertjek</saml:AttributeValue></saml:Attribute></saml:AttributeStatement><saml:AuthenticationStatement AuthenticationMethod=""urn:oasis:names:tc:SAML:1.0:am:password"" AuthenticationInstant=""2016-02-22T12:17:27.241Z""><saml:Subject><saml:NameIdentifier>cvruser1@PROD.DLI</saml:NameIdentifier><saml:SubjectConfirmation><saml:ConfirmationMethod>urn:oasis:names:tc:SAML:1.0:cm:bearer</saml:ConfirmationMethod></saml:SubjectConfirmation></saml:Subject></saml:AuthenticationStatement><ds:Signature xmlns:ds=""http://www.w3.org/2000/09/xmldsig#""><ds:SignedInfo><ds:CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""/><ds:SignatureMethod Algorithm=""http://www.w3.org/2001/04/xmldsig-more#rsa-sha256""/><ds:Reference URI=""#_4c4b13c1-163e-4654-b9ce-38d4898c8140""><ds:Transforms><ds:Transform Algorithm=""http://www.w3.org/2000/09/xmldsig#enveloped-signature""/><ds:Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""/></ds:Transforms><ds:DigestMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#sha256""/><ds:DigestValue>zpDGQML4c2hgVIBuZE21R8q1t3JGsopZI5WKcTL6R3I=</ds:DigestValue></ds:Reference></ds:SignedInfo><ds:SignatureValue>wrNW+puTF7UZz2mGLmGK3AUOQRyS1trl0QSKCSAsOFOsgdj7J1KSyLgrLR/SPAZWFAvcIAnKFPmBgGNa0/PMz+KsF4oUW8YJqpMTMAO9GXDyVQR4k8H4q2sw4ZugX4obW63/AlMOCJchqonsthkuTlHxzYQksSxlflDM7Ct58JaINqp6AF+tTiziR6mrp/j26u/zqfJjWBM0SfESfRET7fDpKAzyT7WTnUvmU6p3cNqtFwgyjDbkeCrugHnxnFEnBtWFegdHIueA4aQcxWAmt5yKoWjcRD2aRqtxyhjzSG82fQMROP1I9PwvV9VcLfaWLy2WCIlxJ1iVrIGC+F0qkA==</ds:SignatureValue><KeyInfo xmlns=""http://www.w3.org/2000/09/xmldsig#""><X509Data><X509Certificate>MIIDkzCCAn+gAwIBAgIQeabFHpwukJJNo/hx3eMN+DAJBgUrDgMCHQUAMEExPzA9BgNVBAMTNkFERlMgU2lnbmluZyAtIGRldnRlc3QtaWRwLWxhbmRtYW5kZGsudmZsdGVzdC5kayBkbGJyMjAgFw0xMjA4MTcwNjUwMTdaGA8yOTk4MTIzMTIyMDAwMFowQTE/MD0GA1UEAxM2QURGUyBTaWduaW5nIC0gZGV2dGVzdC1pZHAtbGFuZG1hbmRkay52Zmx0ZXN0LmRrIGRsYnIyMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAxXf3AdAgxpJcpHgKc2I155whL8qUrIKu3QDEWCkzSLj5owzrxWE7fM/hTINIUWXhimUM2Imt9QIxlqEN+PbZBiwAUrbjE+bueSJRe34kLOSlLK5VxInMCUuvx4xfdF/3B0NDaSv77IM73SZEv1R0jaswz7OIuVzkW8udsxdiFBUYfgjkuI8QefgyUUizZVLJthrXkLIb9vG6/xHixzvoKA+Uwr14JzoDZL5GFqY5Bv2MlIsuYwIqWEkzZotZQ++W2zesUEWIf+nTjrfYOWJx9aKaBgeAdF9kOoZO9+qUL8w15T3QoaHl0fkbP75m7xE5Zui/YVcoHrTproER+bbh4QIDAQABo4GMMIGJMBMGA1UdJQQMMAoGCCsGAQUFBwMBMHIGA1UdAQRrMGmAECb73e9fE3NGkfPzFpdMrN6hQzBBMT8wPQYDVQQDEzZBREZTIFNpZ25pbmcgLSBkZXZ0ZXN0LWlkcC1sYW5kbWFuZGRrLnZmbHRlc3QuZGsgZGxicjKCEHmmxR6cLpCSTaP4cd3jDfgwCQYFKw4DAh0FAAOCAQEAelggKg6ShmJo1JHf+HAkwoWW40RYVAS3k12MU9MowO/OaotHhZ+bHduqMEhgEqqEIklVMo0mV58ofINRaNBGf/oLvQoJTohEvbhq6m/Da9d8kpQBdxG2SwsczsGqSml5FR7FlpYe3gxzdnpXa8cGj9R+OnauNdbU61w336WbiKDTUorh9n+wlOCUckV1ddcClVvwmcyq/zevrbN2euhvQN1+9RopP5hKl5wYPSLUP8OXZuA1X6lFG/QNjAT2wbjkR5UPDGXnollz977VXc1wSLaE1oDBkXF0Rfwafci9OSm4pYwqw61PE33IXq0t9SdGWOzgVFwCy1GOj8mpL7Kxcw==</X509Certificate></X509Data></KeyInfo></ds:Signature></saml:Assertion></trust:RequestedSecurityToken><trust:RequestedAttachedReference><o:SecurityTokenReference xmlns:o=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd""><o:KeyIdentifier ValueType=""http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.0#SAMLAssertionID"">_4c4b13c1-163e-4654-b9ce-38d4898c8140</o:KeyIdentifier></o:SecurityTokenReference></trust:RequestedAttachedReference><trust:RequestedUnattachedReference><o:SecurityTokenReference xmlns:o=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd""><o:KeyIdentifier ValueType=""http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.0#SAMLAssertionID"">_4c4b13c1-163e-4654-b9ce-38d4898c8140</o:KeyIdentifier></o:SecurityTokenReference></trust:RequestedUnattachedReference><trust:TokenType>urn:oasis:names:tc:SAML:1.0:assertion</trust:TokenType><trust:RequestType>http://docs.oasis-open.org/ws-sx/ws-trust/200512/Issue</trust:RequestType><trust:KeyType>http://docs.oasis-open.org/ws-sx/ws-trust/200512/Bearer</trust:KeyType></trust:RequestSecurityTokenResponse></trust:RequestSecurityTokenResponseCollection>";
        const string WSTrust13RstrWithActAsToken = @"<trust:RequestSecurityTokenResponseCollection xmlns:trust=""http://docs.oasis-open.org/ws-sx/ws-trust/200512""><trust:RequestSecurityTokenResponse><trust:Lifetime><wsu:Created xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">2016-02-22T13:18:07.050Z</wsu:Created><wsu:Expires xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">2016-02-22T14:18:07.050Z</wsu:Expires></trust:Lifetime><wsp:AppliesTo xmlns:wsp=""http://schemas.xmlsoap.org/ws/2004/09/policy""><wsa:EndpointReference xmlns:wsa=""http://www.w3.org/2005/08/addressing""><wsa:Address>https://localhost.vfltest.dk/CustomerSampleTier1Service/</wsa:Address></wsa:EndpointReference></wsp:AppliesTo><trust:RequestedSecurityToken><saml:Assertion MajorVersion=""1"" MinorVersion=""1"" AssertionID=""_42a8048d-6948-494f-9467-7e1e7fe949df"" Issuer=""http://si-idp.vfltest.dk/adfs/services/trust"" IssueInstant=""2016-02-22T13:18:07.050Z"" xmlns:saml=""urn:oasis:names:tc:SAML:1.0:assertion""><saml:Conditions NotBefore=""2016-02-22T13:18:07.050Z"" NotOnOrAfter=""2016-02-22T14:18:07.050Z""><saml:AudienceRestrictionCondition><saml:Audience>https://localhost.vfltest.dk/CustomerSampleTier1Service/</saml:Audience></saml:AudienceRestrictionCondition></saml:Conditions><saml:AttributeStatement><saml:Subject><saml:SubjectConfirmation><saml:ConfirmationMethod>urn:oasis:names:tc:SAML:1.0:cm:bearer</saml:ConfirmationMethod></saml:SubjectConfirmation></saml:Subject><saml:Attribute AttributeName=""name"" AttributeNamespace=""http://schemas.xmlsoap.org/ws/2005/05/identity/claims""><saml:AttributeValue>PROD\cvr01</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=""emailaddress"" AttributeNamespace=""http://schemas.xmlsoap.org/ws/2005/05/identity/claims""><saml:AttributeValue>mac@vfl.dk</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=""upn"" AttributeNamespace=""http://schemas.xmlsoap.org/ws/2005/05/identity/claims""><saml:AttributeValue>cvr01@PROD.DLI</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=""givenname"" AttributeNamespace=""http://schemas.xmlsoap.org/ws/2005/05/identity/claims""><saml:AttributeValue> </saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=""surname"" AttributeNamespace=""http://schemas.xmlsoap.org/ws/2005/05/identity/claims""><saml:AttributeValue>cvr01 (89999995)</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=""actor"" AttributeNamespace=""http://schemas.xmlsoap.org/ws/2009/09/identity/claims""><saml:AttributeValue>&lt;Actor&gt;&lt;saml:Attribute AttributeName=""name"" AttributeNamespace=""http://schemas.xmlsoap.org/ws/2005/05/identity/claims"" xmlns:saml=""urn:oasis:names:tc:SAML:1.0:assertion""&gt;&lt;saml:AttributeValue&gt;PROD\customersampleactas&lt;/saml:AttributeValue&gt;&lt;/saml:Attribute&gt;&lt;saml:Attribute AttributeName=""upn"" AttributeNamespace=""http://schemas.xmlsoap.org/ws/2005/05/identity/claims"" xmlns:saml=""urn:oasis:names:tc:SAML:1.0:assertion""&gt;&lt;saml:AttributeValue&gt;customersampleactas@PROD.DLI&lt;/saml:AttributeValue&gt;&lt;/saml:Attribute&gt;&lt;saml:Attribute AttributeName=""emailaddress"" AttributeNamespace=""http://schemas.xmlsoap.org/ws/2005/05/identity/claims"" xmlns:saml=""urn:oasis:names:tc:SAML:1.0:assertion""&gt;&lt;saml:AttributeValue&gt;test@example.com&lt;/saml:AttributeValue&gt;&lt;/saml:Attribute&gt;&lt;saml:Attribute AttributeName=""givenname"" AttributeNamespace=""http://schemas.xmlsoap.org/ws/2005/05/identity/claims"" xmlns:saml=""urn:oasis:names:tc:SAML:1.0:assertion""&gt;&lt;saml:AttributeValue&gt; &lt;/saml:AttributeValue&gt;&lt;/saml:Attribute&gt;&lt;saml:Attribute AttributeName=""surname"" AttributeNamespace=""http://schemas.xmlsoap.org/ws/2005/05/identity/claims"" xmlns:saml=""urn:oasis:names:tc:SAML:1.0:assertion""&gt;&lt;saml:AttributeValue&gt;customersampleactas&lt;/saml:AttributeValue&gt;&lt;/saml:Attribute&gt;&lt;saml:Attribute AttributeName=""authenticationmethod"" AttributeNamespace=""http://schemas.microsoft.com/ws/2008/06/identity/claims"" xmlns:saml=""urn:oasis:names:tc:SAML:1.0:assertion""&gt;&lt;saml:AttributeValue&gt;http://schemas.microsoft.com/ws/2008/06/identity/authenticationmethod/password&lt;/saml:AttributeValue&gt;&lt;/saml:Attribute&gt;&lt;saml:Attribute AttributeName=""authenticationinstant"" AttributeNamespace=""http://schemas.microsoft.com/ws/2008/06/identity/claims"" xmlns:saml=""urn:oasis:names:tc:SAML:1.0:assertion""&gt;&lt;saml:AttributeValue a:type=""tn:dateTime"" xmlns:tn=""http://www.w3.org/2001/XMLSchema"" xmlns:a=""http://www.w3.org/2001/XMLSchema-instance""&gt;2016-02-22T13:18:07.050Z&lt;/saml:AttributeValue&gt;&lt;/saml:Attribute&gt;&lt;/Actor&gt;</saml:AttributeValue></saml:Attribute></saml:AttributeStatement><saml:AuthenticationStatement AuthenticationMethod=""urn:oasis:names:tc:SAML:1.0:am:password"" AuthenticationInstant=""2016-02-22T13:18:06.316Z""><saml:Subject><saml:SubjectConfirmation><saml:ConfirmationMethod>urn:oasis:names:tc:SAML:1.0:cm:bearer</saml:ConfirmationMethod></saml:SubjectConfirmation></saml:Subject></saml:AuthenticationStatement><ds:Signature xmlns:ds=""http://www.w3.org/2000/09/xmldsig#""><ds:SignedInfo><ds:CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""/><ds:SignatureMethod Algorithm=""http://www.w3.org/2001/04/xmldsig-more#rsa-sha256""/><ds:Reference URI=""#_42a8048d-6948-494f-9467-7e1e7fe949df""><ds:Transforms><ds:Transform Algorithm=""http://www.w3.org/2000/09/xmldsig#enveloped-signature""/><ds:Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""/></ds:Transforms><ds:DigestMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#sha256""/><ds:DigestValue>amgJ9Paz/0sTRoRXWd0GrCBGo/mfbihSfI8yF3TZyi8=</ds:DigestValue></ds:Reference></ds:SignedInfo><ds:SignatureValue>PXXU2qrWPkpGte//lWmQdnz9kAIwtZaeywYFVVhDJ+ZbMmL/damPW7Fd4dJAKIy1evF8tm2V6LQKWk48O2sMrdGtYPQY2s33JZWVPI/ZkFnfEgC88zBvJeaPuyarCGIvhMavMyiA2NuB6IBFiL4GZ1AYuIatNA21RqHXagnhWuI4B70z2K5GgH0Lvb/uMAzGRvZGYYeM1HXR3/OExW+bwzP18TC8UY8XcbYmp50FAlRbce4DoewtVfERknolonTlFporS1xOJMee3kM314mltYsI8WOS43FqwLpfbPpnE7h+lzeyBmbaaWyoj7h51qoM22ad2EZ4iNM+8eZrHoPiFQ==</ds:SignatureValue><KeyInfo xmlns=""http://www.w3.org/2000/09/xmldsig#""><X509Data><X509Certificate>MIIE2TCCA8GgAwIBAgISESEcyfemEL+oTtDLjVTeOHU7MA0GCSqGSIb3DQEBBQUAMFcxCzAJBgNVBAYTAkJFMRkwFwYDVQQKExBHbG9iYWxTaWduIG52LXNhMS0wKwYDVQQDEyRHbG9iYWxTaWduIERvbWFpbiBWYWxpZGF0aW9uIENBIC0gRzIwHhcNMTMxMDI5MDgxOTI4WhcNMTYxMDI5MDgxOTI4WjA6MSEwHwYDVQQLExhEb21haW4gQ29udHJvbCBWYWxpZGF0ZWQxFTATBgNVBAMMDCoudmZsdGVzdC5kazCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAKvDI20pFONzxzzv2eLle7ffYnfF/fA8pLp74ccS5AdbmKR6qXsjr8pbPTZlcD4dq/Ezpw1r1nUmDbfJpeIXM5PpXvfkkwCyIcHwRpSsh2FS6PAxTlKZnhlQLbTWFSp/mgDuVltpM9qVWbWvK9sdv90lHVkGjd2kwsAIU9xNnrBQEGLkI3T7QJGAcBvUZUkXnW3c2pFG8W8MHfplG3OSVx9mP0dUDe0YSAs/6sOcATV1Erwv6pVc2ODDgH3bS+n6FHW3xqqs5rn5zJ/zAVefl8m7h27SwGpDYr5JLeqc0ILPbPQ40gXFj2B67krAehTq4ZEToqLKTqTc7rmxx6FRU4sCAwEAAaOCAbowggG2MA4GA1UdDwEB/wQEAwIFoDBJBgNVHSAEQjBAMD4GBmeBDAECATA0MDIGCCsGAQUFBwIBFiZodHRwczovL3d3dy5nbG9iYWxzaWduLmNvbS9yZXBvc2l0b3J5LzAjBgNVHREEHDAaggwqLnZmbHRlc3QuZGuCCnZmbHRlc3QuZGswCQYDVR0TBAIwADAdBgNVHSUEFjAUBggrBgEFBQcDAQYIKwYBBQUHAwIwPwYDVR0fBDgwNjA0oDKgMIYuaHR0cDovL2NybC5nbG9iYWxzaWduLmNvbS9ncy9nc2RvbWFpbnZhbGcyLmNybDCBiAYIKwYBBQUHAQEEfDB6MEEGCCsGAQUFBzAChjVodHRwOi8vc2VjdXJlLmdsb2JhbHNpZ24uY29tL2NhY2VydC9nc2RvbWFpbnZhbGcyLmNydDA1BggrBgEFBQcwAYYpaHR0cDovL29jc3AyLmdsb2JhbHNpZ24uY29tL2dzZG9tYWludmFsZzIwHQYDVR0OBBYEFBzu3R1o3b/yo0nETvNWypgW/3BaMB8GA1UdIwQYMBaAFJat+rBbuYNkKnbCHIpp2kLc/v0oMA0GCSqGSIb3DQEBBQUAA4IBAQBV4q611kljDgOIgy+oW5Yl/Bn7eLjIxN5SxQTXHZA7kyGDL1FIoJjK+fTWLjQZYzULm7pWvFWDfP52fg5RpG0pTLC6AV1cAhfqDeZWsAdTlG/h+m20X/omfm0Jktxw9q/koZgMDUK9twPQHw4Q2wMwO6Molti53HhL6iTxUse1X+e+IqyhwD+CeksGECfDZGaH8hE74HwPosfOzXZAJhBEe1B5IR5nHdjacrTIM3tKe+bTPAkxVnsBhqOzwqBjBpnRnfC+ZEQOiGlYlF8mE8fwjDvBegNdLB/ytYvYfUDnNYcDAXfie5qm8UIxeTG4rQkzaaztYH0abTLexBLn15yL</X509Certificate></X509Data></KeyInfo></ds:Signature></saml:Assertion></trust:RequestedSecurityToken><trust:RequestedAttachedReference><o:SecurityTokenReference xmlns:o=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd""><o:KeyIdentifier ValueType=""http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.0#SAMLAssertionID"">_42a8048d-6948-494f-9467-7e1e7fe949df</o:KeyIdentifier></o:SecurityTokenReference></trust:RequestedAttachedReference><trust:RequestedUnattachedReference><o:SecurityTokenReference xmlns:o=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd""><o:KeyIdentifier ValueType=""http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.0#SAMLAssertionID"">_42a8048d-6948-494f-9467-7e1e7fe949df</o:KeyIdentifier></o:SecurityTokenReference></trust:RequestedUnattachedReference><trust:TokenType>urn:oasis:names:tc:SAML:1.0:assertion</trust:TokenType><trust:RequestType>http://docs.oasis-open.org/ws-sx/ws-trust/200512/Issue</trust:RequestType><trust:KeyType>http://docs.oasis-open.org/ws-sx/ws-trust/200512/Bearer</trust:KeyType></trust:RequestSecurityTokenResponse></trust:RequestSecurityTokenResponseCollection>";


        [Test]
        public void DeserializeTokenFromRstrString_WsTrust13Rstr_CanParsetoken()
        {
            var rstrHelper = new RstrHelper();


            var token = rstrHelper.DeserializeTokenFromRstrString(WSTrust13Rstr);

            Assert.IsInstanceOf<GenericXmlSecurityToken>(token);
            Assert.IsNotNull(token);
            Console.WriteLine(token.Id);
        }

        [Test]
        public void DeserializeTokenFromRstrString_WsTrustvOldRstr_CanParsetoken()
        {
            var rstrHelper = new RstrHelper();

            var token = rstrHelper.DeserializeTokenFromRstrString(WSTrustvOldRstr);

            Assert.IsInstanceOf<GenericXmlSecurityToken>(token);
            Assert.IsNotNull(token);
            Console.WriteLine(token.Id);
        }

        [Test]
        public void RstrHelper_CanRoundtripWsTrustvOldRstr()
        {
            // Old WSTrust roundtrips to the same canonical Xml, WSTrust 1.3 results in modification of SecurityTokenReference, so we compare those via GenericXmlSecurityToken
            var rstrHelper = new RstrHelper();
            RequestSecurityTokenResponse rstr = rstrHelper.DeserializeRstrFromRstrString(WSTrustvOldRstr);
            string rstrString = rstrHelper.SerializeToRstrString(rstr,RstrHelper.WsTrustvOldVersion);
            Assert.That(WSTrustvOldRstr, new XmlSpecificationEqualityConstraint(rstrString));
        }

        [Test]
        public void RstrHelper_CanRoundtripWsTrust13RstrViaGenericXmlSecurityTokens()
        {
            var rstrHelper = new RstrHelper();
            RequestSecurityTokenResponse rstr = rstrHelper.DeserializeRstrFromRstrString(WSTrust13Rstr);
            string rstrString = rstrHelper.SerializeToRstrString(rstr);
            var token = rstrHelper.DeserializeTokenFromRstrString(WSTrust13Rstr);
            var roundTrippedtoken = rstrHelper.DeserializeTokenFromRstrString(rstrString);
            Assert.AreEqual(token.Id,roundTrippedtoken.Id);
        }

        [Test]
        public void RstrHelper_CanRoundtripWsTrust13ActAsRstrViaGenericXmlSecurityTokens()
        {
            var rstrHelper = new RstrHelper();
            RequestSecurityTokenResponse rstr = rstrHelper.DeserializeRstrFromRstrString(WSTrust13RstrWithActAsToken);
            string rstrString = rstrHelper.SerializeToRstrString(rstr);
            var token = rstrHelper.DeserializeTokenFromRstrString(WSTrust13RstrWithActAsToken);
            var roundTrippedtoken = rstrHelper.DeserializeTokenFromRstrString(rstrString);
            Assert.AreEqual(token.Id, roundTrippedtoken.Id);

        }

    }
}