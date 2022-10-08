using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using FullTextSearchSamples.Services;
using FullTextSearchSamples.Services.Impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Linq;

namespace FullTextSearchSamples
{
    internal class Program
    {
        private static IHost host;
        static void Main(string[] args)
        {

            host = Host.CreateDefaultBuilder(args)
                .ConfigureLogging((hostOptions, logOptions) =>
                {
                    logOptions.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
                })
                .ConfigureServices(services =>
                {
                    #region Configure EF DBContext Service

                    services.AddDbContext<DocumentDbContext>(options =>
                    {
                        options.UseSqlServer(
                            connectionString: @"data source=DESKTOP-IA6GVPP\SQLEXPRESS;initial catalog=DocumentsDatabase;User Id=DocumentsDatabaseUser; Password=12345;MultipleActiveResultSets=True;App=EntityFramework"
                        );
                    });

                    #endregion

                    #region Configure Reository

                    services.AddTransient<IDocumentRepository, DocumentRepository>();

                    #endregion
                })
                .Build();

            #region Initial load data.txt to db
            //LoadDocuments()
            #endregion

            // RunSimpleSearch();

            // RunFullTextSearch();

            // RunFullTextAdvancedSearch();

            BenchmarkRunner.Run<PerfComparer>();
        }

        private static void RunFullTextAdvancedSearch()
        {
            var _fullTextIndexAdvancedSearcher = new FullTextIndexAdvancedSearcher();
            var documents = DocumentExtractor.DocumentsSet().ToArray();
            foreach (var document in documents)
                _fullTextIndexAdvancedSearcher.AddStringToIndex(document);

            var sw = new Stopwatch(); sw.Start();
            var e = _fullTextIndexAdvancedSearcher.Search("monday").ToArray();
            sw.Stop();
            foreach (var item in e)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        private static void RunFullTextSearch()
        {
            var fullTextSearcher = new FullTextIndexSearcher();
            var documents = DocumentExtractor.DocumentsSet().ToArray();
            foreach (var document in documents)
                fullTextSearcher.AddStringToIndex(document);
            var e = fullTextSearcher.Search("monday").ToArray();
            foreach (var item in e)
            {
                Console.WriteLine(item);
            }
        }

        private static void RunSimpleSearch()
        {
            var documents = DocumentExtractor.DocumentsSet().ToArray();
            var e = new SimpleSearcher().Search("Monday", documents);
            foreach (var item in e)
            {
                Console.WriteLine(item);
            }
        }

        static void LoadDocuments()
        {
            host.Services
                .GetRequiredService<IDocumentRepository>()
                .LoadDocuments();
        }
    }

    [MemoryDiagnoser]
    [WarmupCount(1)]
    [IterationCount(5)]
    public class PerfComparer
    {
        string[] _documents;
        SimpleSearcher _simpleSearcher;
        FullTextIndexSearcher _fullTextIndexSearcher;
        FullTextIndexAdvancedSearcher _fullTextIndexAdvancedSearcher;


        public PerfComparer()
        {
            _documents = DocumentExtractor.DocumentsSet().ToArray();

            _simpleSearcher = new SimpleSearcher();

            _fullTextIndexSearcher = new FullTextIndexSearcher();
            foreach (var document in _documents)
                _fullTextIndexSearcher.AddStringToIndex(document);

            _fullTextIndexAdvancedSearcher = new FullTextIndexAdvancedSearcher();
            foreach (var document in _documents)
                _fullTextIndexAdvancedSearcher.AddStringToIndex(document);
        }


        [Benchmark(Baseline = true)]
        public void SimpleSearcherBench()
        {
            _simpleSearcher.Search("Monday", _documents).ToArray();
        }

        [Benchmark]
        public void FullTextSearcherBench()
        {
            _fullTextIndexSearcher.Search("monday").ToArray();
        }

        [Benchmark]
        public void FullTextIndexAdvancedSearcherBench()
        {
            _fullTextIndexAdvancedSearcher.Search("monday").ToArray();
        }
    }
}