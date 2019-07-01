using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace SudokuSolver
{
    class Candidate : IEnumerable
    {
        bool[] hodnoty;
        int pocet;
        int pocet_Kandidatu;

        public int Pocet { get { return pocet; } }

        public Candidate(int pocetKandidatu, bool pocatecniHodnota)
        {
            hodnoty = new bool[pocetKandidatu];
            pocet = 0;
            pocet_Kandidatu = pocetKandidatu;

            for (int i = 1; i <= pocetKandidatu; i++)
                this[i] = pocatecniHodnota;
        }

        public bool this[int index]
        {
            get { return hodnoty[index - 1]; }

            // Sleduj pocet kandidatu
            set
            {
                pocet += (hodnoty[index - 1] == value) ? 0 : (value == true) ? 1 : -1;
                hodnoty[index - 1] = value;
            }
        }

        public void NastavVsechnyNaHodnotu(bool hodnota)
        {
            for (int i = 1; i <= pocet_Kandidatu; i++)
                this[i] = hodnota;
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            foreach (int kandidat in this)
                s.Append(kandidat);
            return s.ToString();
        }

        public IEnumerator GetEnumerator()
        {
            return new CandidateEnumerator(this);
        }

        //------------------------------------------//
        //Třída pro průchod foreach cyklem
        private class CandidateEnumerator : IEnumerator
        {
            private int pozice;
            private Candidate c;

            public CandidateEnumerator(Candidate c)
            {
                this.c = c;
                pozice = 0;
            }

            // only iterates over valid candidates
            public bool MoveNext()
            {
                ++pozice;
                if (pozice <= c.pocet_Kandidatu)
                {
                    if (c[pozice] == true)
                        return true;
                    else
                        return MoveNext();
                }
                else
                {
                    return false;
                }
            }

            public void Reset()
            {
                pozice = 0;
            }

            public object Current
            {
                get { return pozice; }
            }
        }
    }
}
