using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Accord;
using Accord.MachineLearning;
using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.MachineLearning.DecisionTrees.Rules;
using Accord.Math;
using Accord.Math.Distances;
using Accord.Math.Optimization.Losses;
using Accord.Statistics.Filters;
using Accord.Statistics.Kernels;


namespace eTracker
{
    public class Learn
    {
        public DecisionTree Tree { get; set; }
        public string Rules { get; set; }
        public string Code { get; set; }

        public string[] InputNames = new[] { "Age", "L1", "Word" };
        public Codification CodeBook { get; set; }



        public Learn()
        {
            try
            {
                //http://accord-framework.net/docs/html/T_Accord_MachineLearning_DecisionTrees_Learning_C45Learning.htm
                using (var db = new DatabaseEntities())
                {
                    var allItems = db.Records.ToList();

                    DataTable data = new DataTable("e-Tracker Values");

                    data.Columns.Add("Id", typeof(int));
                    data.Columns.Add("Age", typeof(string));
                    data.Columns.Add("L1", typeof(string));
                    data.Columns.Add("Word", typeof(string));
                    data.Columns.Add("Synonym", typeof(string));

                    allItems.ForEach(r =>
                    {
                        r.DetailRecords.ToList().ForEach(dr =>
                        {
                            data.Rows.Add(dr.Id, r.Age, r.L1, dr.UnknownWord, dr.SelectedSynonism);
                        });
                    });

                    // Create a new codification codebook to convert 
                    // the strings above into numeric, integer labels:
                    CodeBook = new Codification() {DefaultMissingValueReplacement = Double.NaN};

                    // Learn the codebook
                    CodeBook.Learn(data);

                    // Use the codebook to convert all the data
                    DataTable symbols = CodeBook.Apply(data);

                    // Grab the training input and output instances:
                    int[][] inputs = symbols.ToJagged<int>(InputNames);
                    int[] outputs = symbols.ToArray<int>("Synonym");

                    // Create a new learning algorithm
                    var teacher = new C45Learning()
                    {
                        Attributes = DecisionVariable.FromCodebook(CodeBook, InputNames),
                    };

                    // Use the learning algorithm to induce a new tree:
                    Tree = teacher.Learn(inputs, outputs);

                    // To get the estimated class labels, we can use
                    int[] predicted = Tree.Decide(inputs);

                    // The classification error (~0.214) can be computed as 
                    double error = new ZeroOneLoss(outputs).Loss(predicted);

                    // Moreover, we may decide to convert our tree to a set of rules:
                    DecisionSet rules = Tree.ToRules();

                    // And using the codebook, we can inspect the tree reasoning:
                    string ruleText = rules.ToString(CodeBook, "Synonym",
                        System.Globalization.CultureInfo.InvariantCulture);

                    Rules = ruleText;

                    Code = Tree.ToCode("Rules");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }


        }
        public string Query(string[] queryValues)
        {
            try
            {
                int[] query = CodeBook.Transform(InputNames, queryValues);
                // And then predict the label using
                int predicted = Tree.Decide(query);

                // We can translate it back to strings using
                return CodeBook.Revert("Synonym", predicted);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "Cannot Predict!";
            }
        }
    }
}
