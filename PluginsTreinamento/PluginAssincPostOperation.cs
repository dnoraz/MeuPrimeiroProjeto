using Microsoft.Xrm.Sdk;
using System;

namespace PluginsTreinamento
{
    public class PluginAssincPostOperation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try //tentativa de execuçao
            {
                //Variavel contendo o contexto da execução
                IPluginExecutionContext context =
                    (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                //Variavel contendo o Service Factory da Organização
                IOrganizationServiceFactory serviceFactory =
                    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

                //Variavel contendo o Service Admin que estabelce os serviços de conexao com o Dataverse
                IOrganizationService serviceAdmin = serviceFactory.CreateOrganizationService(null);

                //Variavel do Trace que armazena informaçoes de Log
                ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                //Verifica se contem dados para o destino e se corresponde a uma entity
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    //Variavel do tipo entity herdando a entidade do contexo.
                    Entity entidadeContexto = (Entity)context.InputParameters["Target"];

                    for (int i = 0; i < 10; i++)
                    {
                        //Variavel para nova entidade contato vazia
                        var Contact = new Entity("contact");

                        //atribuiçao dos atributos para novo registro da entidade Contato
                        Contact.Attributes["firstname"] = "Contato Assinc vinculado a Conta";
                        Contact.Attributes["lastname"] = entidadeContexto["name"];
                        Contact.Attributes["parentcustomerid"] = new EntityReference("account", context.PrimaryEntityId);
                        Contact.Attributes["ownerid"] = new EntityReference("systemuser", context.UserId);

                        trace.Trace("firstname: " + Contact.Attributes["firstname"]);

                        serviceAdmin.Create(Contact); //executa o metodo Create para entidade Contato
                    }
                }
            }
            catch (InvalidPluginExecutionException ex) //em caso de falha
            {
                throw new InvalidPluginExecutionException("Erro ocorrido: " + ex.Message); //exibe a Exception
            }
        }
    }
}
