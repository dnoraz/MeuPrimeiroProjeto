using System;
using System.Activities;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace PluginsTreinamento
{
    public class WFValidaLimiteInscricoesAluno : CodeActivity
    {
        #region Parametros
        //recebe o usuario do contexto
        [Input("Usuario")]
        [ReferenceTarget("systemuser")]
        public InArgument<EntityReference> usuarioEntrada { get; set; }

        //recebe o contexto
        [Input("AlunoXCursos")]
        [ReferenceTarget("dio_alunosxcursos")]
        public InArgument<EntityReference> RegistroContexto { get; set; }

        [Output("Saida")]
        public OutArgument<string> saida { get; set; }
        #endregion
        protected override void Execute(CodeActivityContext executionContext)
        {
            //Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService trace = executionContext.GetExtension<ITracingService>();

            //informaçao para o Log de Rastreamento de Plugin!
            trace.Trace("dio_alunosxcursos: " + context.PrimaryEntityId);

            // declaro variavel com o Guid da entidade primaria em uso
            Guid guidAlunoxCursos = context.PrimaryEntityId;

            //informaçao para o Log de Rastreamento de Plugin
            trace.Trace("guidAlunoXCursos: " + guidAlunoxCursos);

            String fetchAlunoXCursos = "<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0'>";
            fetchAlunoXCursos += "<entity name='dio_alunosxcursos' >";
            fetchAlunoXCursos += "<attribute name='dio_alunosxcursosid' />";
            fetchAlunoXCursos += "<attribute name='dio_name' />";
            fetchAlunoXCursos += "<attribute name='dio_emcurso' />";
            fetchAlunoXCursos += "<attribute name='createdon' />";
            fetchAlunoXCursos += "<attribute name='dio_aluno' />";
            fetchAlunoXCursos += "<order descending='false' attribute = 'dio_name' />";
            fetchAlunoXCursos += "<filter type='and' >";
            fetchAlunoXCursos += "<condition attribute ='dio_alunosxcursosid' value = '" + guidAlunoxCursos + "' uitype = 'dio_alunosxcursos' operator = 'eq'/>";
            fetchAlunoXCursos += "</filter> ";
            fetchAlunoXCursos += "</entity>";
            fetchAlunoXCursos += "</fetch>";
            trace.Trace("fetchAlunoXCursos: " + fetchAlunoXCursos);

            var entityAlunoXCursos = service.RetrieveMultiple(new FetchExpression(fetchAlunoXCursos));
            trace.Trace("entityAlunoXCursos: " + entityAlunoXCursos.Entities.Count);

            Guid guidAluno = Guid.Empty;
            foreach (var item in entityAlunoXCursos.Entities)
            {
                string nomeCurso = item.Attributes["dio_name"].ToString();
                trace.Trace("nomeCurso: " + nomeCurso);

                var entityAluno = ((EntityReference)item.Attributes["dio_aluno"]).Id;
                guidAluno = ((EntityReference)item.Attributes["dio_aluno"]).Id;
                trace.Trace("entityAluno: " + entityAluno);
            }

            String fetchAlunoXCursosQtde = "<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0'>";
            fetchAlunoXCursosQtde += "<entity name='dio_alunosxcursos' >";
            fetchAlunoXCursosQtde += "<attribute name='dio_alunosxcursosid' />";
            fetchAlunoXCursosQtde += "<attribute name='dio_name' />";
            fetchAlunoXCursosQtde += "<attribute name='dio_aluno' />";
            fetchAlunoXCursosQtde += "<attribute name='createdon' />";
            fetchAlunoXCursosQtde += "<order descending= 'false' attribute = 'dio_name' />";
            fetchAlunoXCursosQtde += "<filter type='and' >";
            fetchAlunoXCursosQtde += "<condition attribute='dio_aluno' value = '" + guidAluno + "' operator= 'eq' />";
            fetchAlunoXCursosQtde += "</filter>";
            fetchAlunoXCursosQtde += "</entity>";
            fetchAlunoXCursosQtde += "</fetch>";
            trace.Trace("fetchAlunoXCursosQtde: " + fetchAlunoXCursosQtde);
            var entityAlunoXCursosQtde = service.RetrieveMultiple(new FetchExpression(fetchAlunoXCursosQtde));
            trace.Trace("entityAlunoXCursosQtde: " + entityAlunoXCursosQtde.Entities.Count);

            if(entityAlunoXCursosQtde.Entities.Count > 2)
            {
                saida.Set(executionContext, "Aluno excedeu o limite de cursos ativos!");
                trace.Trace("Aluno excedeu o limite de cursos ativos!");
                throw new InvalidPluginExecutionException("Aluno excedeu o limite de cursos ativos!");
            }
        }
    }
}
