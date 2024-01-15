using System;
using Microsoft.Xrm.Sdk;

namespace PluginsTreinamento
{
    public class PluginAccountPostOperation : IPlugin
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

                    if (!entidadeContexto.Contains("websiteurl")) //verifica se o atributo websiteurl nao esta presente no contexto
                    {
                        throw new InvalidPluginExecutionException("Campo websiteurl principal é obrigatório!"); //exibe Exception de erro
                    }

                    // Variavel para nova entidade TASK vazia
                    var Task = new Entity("task");

                    //atribuiçao dos atributos para novo registro da entidade TASK
                    Task.Attributes["ownerid"] = new EntityReference("systemuser", context.UserId);
                    Task.Attributes["regardingobjectid"] = new EntityReference("account", context.PrimaryEntityId);
                    Task.Attributes["subject"] = "Visite nosso site: " + entidadeContexto["websiteurl"];
                    Task.Attributes["description"] = "TASK criada via Plugin Post Operation";

                    serviceAdmin.Create(Task); //executa o metodo CREATE para entidade TASK
                }
            }
            catch (InvalidPluginExecutionException ex) //em caso de falha
            {
                throw new InvalidPluginExecutionException("Erro ocorrido: " + ex.Message); //exibe a exception 
            }
        }
    }
}
