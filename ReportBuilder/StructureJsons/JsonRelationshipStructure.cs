using System;
using System.Collections.Generic;

namespace Cabronate.DAO.ReportBuilder.StructureJsons
{
    public class JsonRelationshipStructure
    {
        public JsonRelationshipStructure(string tableTarget)
        {
            TableTarget = tableTarget;
            Links = new List<JsonLinksStructure>();
        }

        public string TableTarget { get; set; }        

        public List<JsonLinksStructure> Links { get; set; }

        public override bool Equals(Object obj)
        {
            bool equal = true;
            JsonRelationshipStructure otherRelation = (obj as JsonRelationshipStructure);
            if (otherRelation == null) return false;

            equal &= (TableTarget.ToUpper() == otherRelation.TableTarget.ToUpper());
            equal &= (Links.Count == otherRelation.Links.Count);

            bool hasLink = false;
            otherRelation.Links.ForEach(otherRelationLink =>
            {
                Links.ForEach(localLink =>
                {
                    if (localLink.TableTargetConnectoinField.ToUpper() == otherRelationLink.TableTargetConnectoinField.ToUpper() &&
                        localLink.SelfConnectionField.ToUpper() == otherRelationLink.SelfConnectionField.ToUpper())
                        hasLink = true;
                });
                equal &= hasLink;
            });
            return equal;
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}
