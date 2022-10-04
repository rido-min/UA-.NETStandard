using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Configuration;

namespace ClientSample
{
    internal class ConnectionConfig
    {
        static internal ApplicationConfiguration Create(string url)
        {
            ApplicationConfiguration config = new ApplicationConfiguration() {
                ApplicationType = ApplicationType.Client,
                ApplicationName = "Consle OPC Client",
                SecurityConfiguration = new SecurityConfiguration() {
                    ApplicationCertificate = new CertificateIdentifier() {
                        StoreType = @"Directory",
                        StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\MachineDefault",
                        SubjectName = "MyOpcClient"
                    },
                    TrustedIssuerCertificates = new CertificateTrustList {
                        StoreType = @"Directory",
                        StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Certificate Authorities"
                    },
                    TrustedPeerCertificates = new CertificateTrustList {
                        StoreType = @"Directory",
                        StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Applications"
                    },
                    RejectedCertificateStore = new CertificateTrustList {
                        StoreType = @"Directory",
                        StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\RejectedCertificates"
                    },
                    AutoAcceptUntrustedCertificates = true
                },
                TransportConfigurations = new TransportConfigurationCollection(),
                TransportQuotas = new TransportQuotas { OperationTimeout = (int)TimeSpan.FromMinutes(10).TotalMilliseconds },
                ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds }
            };
            config.Validate(ApplicationType.Client).GetAwaiter().GetResult();
            config.CertificateValidator.CertificateValidation += (s, e) => e.AcceptAll = true;

            ApplicationInstance app = new ApplicationInstance(config);
            app.CheckApplicationInstanceCertificate(false, 2048).GetAwaiter().GetResult();
            return config;
        }
    }
}
