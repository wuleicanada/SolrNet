﻿using System;
using Xunit;
using Ninject.Integration.SolrNet.Config;
using SolrNet;
using Xunit.Abstractions;

namespace Ninject.Integration.SolrNet.Tests {
    
    [Trait("Category","Integration")]
    public class NinjectIntegrationFixture {
        private readonly ITestOutputHelper testOutputHelper;
        private StandardKernel kernel;

        public NinjectIntegrationFixture(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            kernel = new StandardKernel();
        }

        [Fact]
        public void Ping_And_Query()
        {
            var c = new StandardKernel();
            c.Load(new SolrNetModule("http://localhost:8983/solr/techproducts"));
            var solr = c.Get<ISolrOperations<NinjectFixture.Entity>>();
            solr.Ping();
            testOutputHelper.WriteLine(solr.Query(SolrQuery.All).Count.ToString());
        }

        [Fact]
        public void Ping_And_Query_SingleCore()
        {
            var solrServers = new SolrServers {
                new SolrServerElement {
                    Id = "default",
                    Url = "http://localhost:8983/solr/techproducts/core0",
                    DocumentType = typeof(Entity).AssemblyQualifiedName,
                }
            };
            kernel.Load(new SolrNetModule(solrServers));
            var solr = kernel.Get<ISolrOperations<Entity>>();
            solr.Ping();
            testOutputHelper.WriteLine(solr.Query(SolrQuery.All).Count.ToString());
        }

        [Fact]
        public void Ping_And_Query_MultiCore()
        {
            var solrServers = new SolrServers {
                new SolrServerElement {
                    Id = "main",
                    Url = "http://localhost:8983/solr/techproducts/core0",
                    DocumentType = typeof(Entity).AssemblyQualifiedName,
                },
                new SolrServerElement {
                    Id = "alt",
                    Url = "http://localhost:8983/solr/techproducts/core1",
                    DocumentType = typeof(Entity2).AssemblyQualifiedName,
                }
            };
            kernel.Load(new SolrNetModule(solrServers));
            var solr1 = kernel.Get<ISolrOperations<Entity>>();
            solr1.Ping();
            testOutputHelper.WriteLine("Query core 1: {0}", solr1.Query(SolrQuery.All).Count);
            var solr2 = kernel.Get<ISolrOperations<Entity2>>();
            solr2.Ping();
            testOutputHelper.WriteLine("Query core 2: {0}", solr2.Query(SolrQuery.All).Count);
        }

        [Fact]
        public void MultiCore_GetByName()
        {
            var solrServers = new SolrServers {
                new SolrServerElement {
                    Id = "core-0",
                    Url = "http://localhost:8983/solr/techproducts/core0",
                    DocumentType = typeof(Entity).AssemblyQualifiedName,
                },
                new SolrServerElement {
                    Id = "core-1",
                    Url = "http://localhost:8983/solr/techproducts/core1",
                    DocumentType = typeof(Entity2).AssemblyQualifiedName,
                }
            };
            kernel.Load(new SolrNetModule(solrServers));
            var solr1 = kernel.Get<ISolrOperations<Entity>>("core-0");
            Assert.NotNull(solr1);
            testOutputHelper.WriteLine("Query core 1: {0}", solr1.Query(SolrQuery.All).Count);
            var solr2 = kernel.Get<ISolrOperations<Entity2>>("core-1");
            Assert.NotNull(solr2);
            solr2.Ping();
            testOutputHelper.WriteLine("Query core 2: {0}", solr2.Query(SolrQuery.All).Count);
        }

    }
}
