using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Libro.Models;

namespace Libro
{
    class DataGridEx : DependencyObject
    {
        public static readonly DependencyProperty SaveOnCommitProperty = DependencyProperty.RegisterAttached(
            "SaveOnCommit", typeof(bool), typeof(DataGridEx), new PropertyMetadata(default(bool),OnCommitChanged));

        public static void SetSaveOnCommit(DependencyObject element, bool value)
        {
            element.SetValue(SaveOnCommitProperty, value);
        }

        public static bool GetSaveOnCommit(DependencyObject element)
        {
            return (bool) element.GetValue(SaveOnCommitProperty);
        }


        private static void OnCommitChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var dg = dependencyObject as DataGrid;
            if (dg == null) return;
            dg.RowEditEnding += (sender, args) =>
            {
                if (args.EditAction != DataGridEditAction.Commit) return;
                args.Row.BindingGroup.UpdateSources();
                var model =(args.Row.DataContext as IModel);
                if (model == null) return;
                if (!model.IsValid)
                {
                    args.Cancel = true;
                    return;
                }
                model.Save();
            };
        }

    }

    //public static class ObservableCollectionExtension
    //{
    //    /// <summary>
    //    /// Extension method on the Observable collection for enabling sorting mechanism
    //    /// </summary>
    //    /// <param name="source"></param>
    //    /// <param name="selector"></param>
    //    /// <param name="direction"></param>
    //    /// <typeparam name="TSource"></typeparam>
    //    /// <typeparam name="TValue"></typeparam>
    //    public static void Sort<TSource, TValue>(this ObservableCollection<TSource> source, Func<TSource, TValue> selector, ListSortDirection? direction)
    //    {
    //        for(int i = source.Count - 1; i >= 0; i--)
    //        {
    //            for(int j = 1; j <= i; j++)
    //            {
    //                var row1 = source.ElementAt(j - 1);
    //                var row2 = source.ElementAt(j);

    //                //// do not account for new rows. keep them out of sorting logic ////
    //                if((row1 as RowViewModel).IsNewRow || (row2 as RowViewModel).IsNewRow)
    //                {
    //                    break;
    //                }

    //                var cell1 = selector(row1);
    //                var cell2 = selector(row2);
    //                int sortResult = (cell1 as CellViewModel).CompareTo(cell2 as CellViewModel);

    //                sortResult = direction == ListSortDirection.Ascending ? sortResult * 1 : sortResult * -1;

    //                if(sortResult > 0)
    //                {
    //                    // Position the item correctly
    //                    source.Move(j - 1, j);
    //                }
    //            }
    //        }
    //    }
    //}
}
