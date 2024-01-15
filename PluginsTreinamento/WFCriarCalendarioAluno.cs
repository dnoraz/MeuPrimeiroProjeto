using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk;
using System;
using System.Activities;
using Microsoft.Xrm.Sdk.Query;

namespace PluginsTreinamento
{
    public class WFCriarCalendarioAluno : CodeActivity
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

            // declaro variavel com o Guid da entidade primaria em uso
            Guid guidAlunoxCursos = context.PrimaryEntityId;

            //informaçao para o Log de Rastreamento de Plugin
            trace.Trace("guidAlunoXCursos: " + guidAlunoxCursos);

            //Busca informaçoes sobre o curso do contexto
            Entity entityAlunoXCursos = service.Retrieve("dio_alunosxcursos", guidAlunoxCursos,
                new ColumnSet("dio_cursoselecionado", "dio_periodo", "dio_datadeinicio"));

            //declara uma variavel Guid Null
            Guid guidCurso = Guid.Empty;

            //variavel periodo do curso
            var PeriodoCurso = String.Empty;

            //variavel Data de Inicio
            DateTime dataInicio = new DateTime();

            //se retornou o campo periodo
            if(entityAlunoXCursos != null)
            {
                guidCurso = ((EntityReference)entityAlunoXCursos.Attributes["dio_cursoselecionado"]).Id;
                trace.Trace("guidCurso: " + guidCurso);
                if (entityAlunoXCursos.Attributes.Contains("dio_periodo"))
                {
                    trace.Trace("Periodo: " + ((OptionSetValue)entityAlunoXCursos["dio_periodo"]).Value);
                    if (((OptionSetValue)entityAlunoXCursos["dio_periodo"]).Value == 914300000)
                    {
                        PeriodoCurso = "Diurno";
                        trace.Trace("Periodo Diurno");
                    }
                    else
                    {
                        PeriodoCurso = "Noturno";
                        trace.Trace("Periodo Noturno");
                    }
                }
                if (entityAlunoXCursos.Attributes.Contains("dio_datadeinicio"))
                {
                    DateTime varDataInicio = ((DateTime)entityAlunoXCursos["dio_datadeinicio"]);
                    dataInicio = new DateTime(varDataInicio.Year, varDataInicio.Month, varDataInicio.Day);
                    trace.Trace("dataInicio: " + dataInicio);
                    trace.Trace("Dia da Semana: " + dataInicio.ToString("ddd"));
                }
            }

            //Se retornou o Guid do Curso
            if(guidCurso != Guid.Empty)
            {
                Entity entityCurso = service.Retrieve("dio_cursos", guidCurso, new ColumnSet("dio_duracaodocurso"));
                int horasDuracao = 0;
                if(entityCurso != null && entityCurso.Attributes.Contains("dio_duracaodocurso"))
                {
                    horasDuracao = Convert.ToInt32(entityCurso.Attributes["dio_duracaodocurso"].ToString());
                }
                trace.Trace("horasDuracao: " + horasDuracao);

                //contagem dos dias necessarios
                int diasNecessarios = 0;
                if(PeriodoCurso == "Diurno")
                {
                    //contagem do numero de dias necessarios para o curso (Duraçao em Horas/ 8 horas diarias) Diurno
                    diasNecessarios = horasDuracao / 8;
                    trace.Trace("diasNecessarios: " + diasNecessarios);
                }
                else if(PeriodoCurso == "Noturno")
                {
                    //contagem do numero de dias necessarios para o curso (Duraçao em Horas/ 4 horas diarias) Noturno
                    diasNecessarios = horasDuracao / 4;
                    trace.Trace("diasNecessarios: " + diasNecessarios);
                }

                //Cria o calendario do aluno
                if(diasNecessarios > 0)
                {
                    for(int i = 0; i < diasNecessarios; i++)
                    {
                        //valida se o dia da semana é um sabado em caso de periodo Noturno
                        if(dataInicio.ToString("ddd") == "Sat" && PeriodoCurso == "Noturno")
                        {
                            dataInicio = dataInicio.AddDays(2);
                        }
                        Entity entCalendarioAluno = new Entity("dio_calendariodoaluno");
                        entCalendarioAluno["dio_name"] = "Aula do dia" + dataInicio.ToString("ddd") + " - " + dataInicio;
                        entCalendarioAluno["dio_data"] = dataInicio;
                        entCalendarioAluno["dio_alunoxcursodisponivel"] = new EntityReference("dio_alunosxcursos", guidAlunoxCursos);
                        service.Create(entCalendarioAluno);

                        trace.Trace("Aula: " + i.ToString() + " - Data: " + dataInicio);

                        dataInicio = dataInicio.AddDays(1);
                    }
                }
            }
        }
    }
}
