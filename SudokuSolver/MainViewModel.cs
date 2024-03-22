using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SudokuSolver
{
    public class SolutionLegendItem
    {
        public string SolutionStrategieName { get; set; }
        public SolutionStrategie SolutionStrategie { get; set; }
        public SolidColorBrush Color { get; set; }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName]string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private List<SolutionLegendItem> _solutionLegendItems;

        public List<SolutionLegendItem> SolutionLegendItems
        {
            get { return _solutionLegendItems; }
            set
            {
                _solutionLegendItems = value;
                RaisePropertyChanged();
            }
        }

        private int? _iterations;

        public int? Iterations
        {
            get { return _iterations; }
            set
            {
                _iterations = value;
                RaisePropertyChanged();
            }
        }


        public void SetSolutionLegend(IEnumerable<SolutionStrategie> strategies, Dictionary<SolutionStrategie, Color> colors, int? iterations)
        {
            List<SolutionLegendItem> legend = new List<SolutionLegendItem>();

            foreach (var strategy in strategies.OrderBy(x => x))
            {
                legend.Add(new SolutionLegendItem
                {
                    Color = new SolidColorBrush(colors[strategy]),
                    SolutionStrategie = strategy,
                    SolutionStrategieName = strategy.ToString()
                });
            }

            SolutionLegendItems = legend;
            Iterations = iterations;
        }

    }
}
