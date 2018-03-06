using System;
using System.Threading.Tasks;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Commons.Domain.Interfaces;
using MidnightLizard.Commons.Domain.Results;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using MidnightLizard.Schemes.Infrastructure.Configuration;
using Nest;
using MidnightLizard.Schemes.Infrastructure.Versioning;

namespace MidnightLizard.Schemes.Infrastructure.Snapshot
{
    public class SchemesSnapshotAccessor : AggregateSnapshotAccessor<PublicScheme, PublicSchemeId>
    {
        protected override string IndexName => config.ELASTIC_SEARCH_SNAPSHOT_SCHEMES_INDEX_NAME;
        protected override PublicScheme CreateNewAggregate(PublicSchemeId id) => new PublicScheme(id);

        public SchemesSnapshotAccessor(AppVersion version, ElasticSearchConfig config) : base(version, config)
        {
        }

        public override async Task Save(AggregateSnapshot<PublicScheme, PublicSchemeId> schemeSnapshot)
        {
            var result = await this.elasticClient.UpdateAsync<PublicScheme, object>(
                new DocumentPath<PublicScheme>(schemeSnapshot.Aggregate.Id.Value),
                u => u.Doc(new
                {
                    Version = version.ToString(),
                    schemeSnapshot.RequestTimestamp,
                    schemeSnapshot.Aggregate.PublisherId,
                    schemeSnapshot.Aggregate.ColorScheme
                }).DocAsUpsert());
        }

        protected override IPromise<IMappings> ApplyAggregateMappingsOnIndex(MappingsDescriptor md)
        {
            return md.Map<PublicScheme>(tm => tm
                .Properties(prop => prop
                    .Keyword(x => x.Name(nameof(Version)))
                    .Date(x => x.Name(nameof(AggregateSnapshot<PublicScheme, PublicSchemeId>.RequestTimestamp)))
                    .Keyword(x => x.Name(n => n.PublisherId))
                    .Object<ColorScheme>(cs => cs
                        .Name(x => x.ColorScheme)
                        .AutoMap()
                        .Properties(eProp => eProp
                            .Keyword(x => x.Name(n => n.colorSchemeId))
                            .Keyword(x => x.Name(n => n.colorSchemeName))))));
        }
    }
}