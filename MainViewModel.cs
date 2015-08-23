using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SudokuSolver.Annotations;

namespace SudokuSolver {
    public class MainViewModel:INotifyPropertyChanged {
        private List<List<int>> _jogo;
        public List<List<int>> Jogo {
            get { return _jogo; }
            set {
                if (value == _jogo)
                    return;
                _jogo = value;
                OnPropertyChanged("Jogo");
            }
        }
        public MainViewModel() {
            
        }
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public List<List<int>> ResolveJogo(List<List<int>> jogo = null) {
            var a = ResolveMetodoA(jogo ?? Jogo);
            //if (TemZeros(a)) {
            //    a = ResolveMetodoB(a);
            //}
            //if (TemZeros(a)) return ResolveJogo(a);
            return a;
        }
        //private List<List<int>> ResolveMetodoB() {
        //    for (int i = 0; i < 9; i++) {
        //        for (int j = 0; j < 9; j++) {
        //            if (Jogo[i][j] > 0) continue;

        //            var inter = Possibilidades(j, i).ToList();
        //            if (inter.Count == 1) {
        //                Jogo[i][j] = inter[0];
        //                return ResolveJogo();
        //            }
        //        }
        //    }


        //}
        public List<List<int>> ResolveMetodoA(List<List<int>> jogo) {
            var game = jogo;
            for (int i = 0; i < 9; i++) {
                for (int j = 0; j < 9; j++) {
                    if (game[i][j] > 0) continue;
                    var inter = Possibilidades(j, i, game).ToList();
                    if(inter.Count == 0) throw new Exception("Sem possibilidades");
                    if (inter.Count == 1) {
                        game[i][j] = inter[0];
                        return ResolveMetodoA(game);
                    }
                }
            }

            //if (TemZeros(game)) {
            //    for (int i = 0; i < 9; i++) {
            //        for (int j = 0; j < 9; j++) {
            //            if (game[i][j] > 0) continue;
            //            var inter = Possibilidades(j, i, game).ToList();
            //            foreach (var i1 in inter) {
            //                var newgame = CopiaJogo(game);// new List<List<int>>(game);
            //                newgame[i][j] = i1;
            //                try {
            //                    return ResolveMetodoA(newgame);

            //                } catch (Exception) {
                                
            //                }
            //            }
                       
            //        }
            //    }
            //}

            //if (TemZeros(game))
            //    throw new Exception("Fim das tentativas");



            return game;
        }
        
        public List<List<int>> CopiaJogo(List<List<int>> jogo) {
            var result = new List<List<int>>();
            foreach (var linha in jogo) {
                result.Add(new List<int>());
                foreach (int j in linha) {
                    result.Last().Add(j);
                }
                
            }
            return result;
        }
        //private List<List<int>> ResolveMetodoB(List<List<int>> game) {
        //    for (int i = 0; i < 9; i++) {
        //        for (int j = 0; j < 9; j++) {
        //            if (game[i][j] > 0) continue;
        //            var inter = Possibilidades(j, i, game).ToList();
        //            if (inter.Count == 0) throw new Exception("Sem possibilidades");
        //            foreach (var i1 in inter) {
                        
        //            }
        //        }
        //    }
           
            
        //}
        public bool TemZeros(List<List<int>> jogo) {
            return jogo.Any(l => l.Any(i => i == 0));
        }

        public bool TemImpossiilidade(List<List<int>>  jogo) {
            var solve = ResolveJogo(jogo);
            return true;
        }

        public List<int> PossibilidadesRow(int row, List<List<int>> jogo = null) {
            var game = jogo ?? Jogo;
            int[] arrayRow = new int[9];
            for (var x = 0; x < 9; x++) {
                arrayRow[x] = game[x][ row];
            }

            return ItensFaltantes(arrayRow);

            

        }
        public List<int> PossibilidadesCol(int col, List<List<int>> jogo = null) {
            var game = jogo ?? Jogo;
            
            int[] arrayRow = new int[9];
            for (var x = 0; x < 9; x++) {
                arrayRow[x] = game[col][x];
            }

            return ItensFaltantes(arrayRow);

        }
        public void ConstroiJogo(List<List<int>> linhasJogo) {
           Jogo = new List<List<int>>();
            for (int i = 0; i < 9; i++) {
                Jogo.Add(new List<int>());
                for (int j = 0; j < 9; j++) {
                    Jogo[i].Add(linhasJogo[j][i]);
                }
            }
            
        }
        public int GetSquareColRow(int colRow) {
            var div = Convert.ToDecimal(colRow) / 3;
            var coluna =Convert.ToInt32(Math.Floor(div)*3);
            return coluna;
        }

        public List<int> PossibilidadesSquare(int squareCol, int squareRow, List<List<int>> jogo = null) {
            var game = jogo ?? Jogo;
            var colStart = GetSquareColRow(squareCol);
            var rowStart = GetSquareColRow(squareRow);

            int[] itens = new int[9];
            int count = 0;
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    itens[count++] = game[colStart + i][ rowStart + j];
                }
            }

            var faltantes = ItensFaltantes(itens);
            return  faltantes;
        }

        public int[] Possibilidades(int row, int col, List<List<int>> jogo = null ) {
            var posRow = PossibilidadesRow(row,jogo);
            var posCol = PossibilidadesCol(col,jogo);
            var posSqr = PossibilidadesSquare(col, row,jogo);
            var inter = posRow.Intersect(posCol).Intersect(posSqr).ToList();
            return inter.ToArray();


        }
        
        public List<int> ItensFaltantes(int[] itens) {
            var itensTotal = new[] {1, 2, 3, 4, 5, 6, 7, 8, 9};
            var result = itensTotal.Except(itens);
            return result.ToList();

        }






    }
}
