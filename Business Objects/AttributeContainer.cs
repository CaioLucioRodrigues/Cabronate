using Cabronate.Base;
using Cabronate.DAO.Attributes;
using Cabronate.DAO.Operations;
using Cabronate.DAO.Value_Objects;
using Cabronate.DAO.View_Models;
using System;
using System.Collections.Generic;

namespace Cabronate.DAO.Business_Objects
{
    [Serializable]
    public sealed class AttributeContainer
    {
        private readonly EcalcValueObjectDAO _vo;

        public AttributeContainer(EcalcValueObjectDAO vo)
        {
            Attributes = new List<AtributoRespondidoBO>();
            AttributesViewModel = new List<AttributeViewModel>();
            _vo = vo;
        }

        private List<AtributoRespondidoBO> Attributes { get; }

        public List<AttributeViewModel> AttributesViewModel { get; }

        public void Load(DBContexto dbctx)
        {
            Load(dbctx, _vo.KeyFieldValue.ToString());
        }

        public void Load(DBContexto dbctx, string entidadeId)
        {
            Attributes.Clear();
            Attributes.AddRange(AtributoRespondidoBO.CarregaListaPorEntidade(
                dbctx,
                AttributeReaderSingleton.AttributeReader.GetOrigin(_vo),
                entidadeId));

            AttributesViewModel.Clear();
            Attributes.ForEach(answeredAttribute =>
            {
                AttributesViewModel.Add(new AttributeViewModel()
                {
                    Id = answeredAttribute.Id,
                    Observation = answeredAttribute.Observacao,
                    Name = GenericOP.GetFieldValue<AtributoBO>(dbctx, answeredAttribute.AtributoId, "ATRIBUTO"),
                    AttributeId = answeredAttribute.AtributoId,
                    AnswerId = answeredAttribute.RespostaId,
                    Answer = GetTextAnswer(dbctx, answeredAttribute)
                });
            });
        }

        private string GetTextAnswer(DBContexto dbctx, AtributoRespondidoBO attribute)
        {
            switch (attribute.TipoResposta)
            {
                case TipoDeResposta.Lista:
                    return GenericOP.GetFieldValue<AtributoRespostaBO>(dbctx, attribute.RespostaId, "RESPOSTA");
                case TipoDeResposta.Texto:
                    return attribute.RespostaTexto;
                case TipoDeResposta.Numerico:
                    return attribute.RespostaNumero.ToString();
                case TipoDeResposta.Data:
                    return attribute.RespostaData.ToString(EcalcConsts.BR_DATE_FORMAT);
                case TipoDeResposta.DataEHora:
                    return attribute.RespostaData.ToString(EcalcConsts.BR_DATE_TIME_FORMAT);
            }
            return "";
        }
    }
}

