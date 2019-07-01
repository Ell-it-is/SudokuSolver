using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    struct Bunka
    {
        public int Radek { get; }
        public int Sloupec { get; }

        public Bunka(int r, int c)
        {
            Radek = r;
            Sloupec = c;
        }
    }

    class SudokuSolver
    {
        int[,] sudokuResene;

        //Najít vyřešené pole a vytvořit omezení pro ostatní pole
        Candidate[,] omezeniBunek; //9x9 pole(všechny buňky), každá bude mít třídu kandidáta určující jaké hodnoty jsou povoleny(9 boolean hodnot).
        Candidate[] omezeniRadku; //Proměnná je pole o 9 hodnotách(9 řádků). Říká nám jaké hodnoty jsou zaznamenány v každém řádku.
        Candidate[] omezeniSloupce;
        Candidate[,] omezeniRegionu; //Záznam toho jaké hodnoty byly zapsány v daném regionu (3x3)
        HashSet<Bunka> nevyresene;
        HashSet<Bunka> vyresene;
        Stack<HashSet<Bunka>> zmenene;
        int pocetKroku = 0;

        public SudokuSolver(int[,] sudokuZadani)
        {
            sudokuResene = sudokuZadani;
            omezeniBunek = new Candidate[9, 9];
            omezeniRadku = new Candidate[9];
            omezeniSloupce = new Candidate[9];
            omezeniRegionu = new Candidate[9, 9];
            nevyresene = new HashSet<Bunka>();
            vyresene = new HashSet<Bunka>();
            zmenene = new Stack<HashSet<Bunka>>();

            //Vytvořit všechny kandidáty pro každé pole aby neměli hodnotu null
            for (int radek = 0; radek < 9; radek++)
                for (int sloupec = 0; sloupec < 9; sloupec++)
                    omezeniBunek[radek, sloupec] = new Candidate(9, true);

            for (int i = 0; i < 9; i++)
            {
                omezeniRadku[i] = new Candidate(9, false);
                omezeniSloupce[i] = new Candidate(9, false);
            }

            //Jednotlivé omezení na regiony budou uloženy v buňkách o rozměrech pole 3x3
            //Každá buňka představuje jeden region
            for (int radek = 0; radek < 3; radek++)
                for (int sloupec = 0; sloupec < 3; sloupec++)
                    omezeniRegionu[radek, sloupec] = new Candidate(9, false);

            InicializaceOmezeni();
            OmezitKandidaty();
        }

        private void InicializaceOmezeni()
        {
            //1) Vyber prvky ze zadání jež nemají 0, tedy jsou vyřešené
            for (int radek = 0; radek < 9; radek++)
            {
                for (int sloupec = 0; sloupec < 9; sloupec++)
                {
                    //Vyber vyřešené pole
                    if (sudokuResene[radek, sloupec] > 0)
                    {
                        //Ulož hodnotu vyřešeného pole
                        int cislo = sudokuResene[radek, sloupec];
                        //Zařadím ho do omezení ř,s,r
                        omezeniRadku[radek][cislo] = true; //První [] je index pole, druhé [] je indexor třídy Candidate
                        omezeniSloupce[sloupec][cislo] = true;
                        omezeniRegionu[radek / 3, sloupec / 3][cislo] = true;
                    }
                }
            }
        }

        //Dosazeni omezeni do nevyresenych poli
        private void OmezitKandidaty()
        {
            for (int radek = 0; radek < 9; radek++)
            {
                for (int sloupec = 0; sloupec < 9; sloupec++)
                {
                    //V případě, že pole je vyřešené
                    if (sudokuResene[radek, sloupec] > 0)
                    {
                        //Vyřešené pole nepotřebuje kandidáty
                        omezeniBunek[radek, sloupec].NastavVsechnyNaHodnotu(false);
                        vyresene.Add(new Bunka(radek, sloupec));
                    }
                    else
                    {
                        //Najdu nevyřešené číslo a chci mu přiřadit omezení na kandidáty
                        //1) Podívám se jestli tam vůbec nějaké omezení je:
                        //2) Pokud ano, překopíruji číslo z omezení a v kandidátu buňky ho nastavým jako neplatné
                        foreach (int cislo in omezeniRadku[radek])
                        {
                            omezeniBunek[radek, sloupec][cislo] = false;
                        }

                        foreach (int cislo in omezeniSloupce[sloupec])
                        {
                            omezeniBunek[radek, sloupec][cislo] = false;
                        }


                        foreach (int cislo in omezeniRegionu[radek / 3, sloupec / 3])
                        {
                            omezeniBunek[radek, sloupec][cislo] = false;
                        }

                        Bunka b = new Bunka(radek, sloupec);
                        nevyresene.Add(b);
                    }
                }
            }
        }

        public Bunka DalsiBunka()
        {
            pocetKroku++;
            if (nevyresene.Count == 0)
            {
                return new Bunka(-1, -1);
            }

            Bunka min = nevyresene.First();
            foreach (Bunka bunka in nevyresene)
                min = (omezeniBunek[bunka.Radek, bunka.Sloupec].Pocet < omezeniBunek[min.Radek, min.Sloupec].Pocet) ? bunka : min;

            return min;
        }

        public void OdeberKandidata(Bunka bunka, int kandidat)
        {
            sudokuResene[bunka.Radek, bunka.Sloupec] = 0;

            //Vypnout omezení Řádek, Sloupec, Region
            omezeniRadku[bunka.Radek][kandidat] = false;
            omezeniSloupce[bunka.Sloupec][kandidat] = false;
            omezeniRegionu[bunka.Radek / 3, bunka.Sloupec / 3][kandidat] = false;

            //Nastavi cislo zpet jako vhodneho kandidata pro zmenene bunky
            omezeniBunek[bunka.Radek, bunka.Sloupec][kandidat] = true;
            foreach (Bunka b in zmenene.Pop())
            {
                omezeniBunek[b.Radek, b.Sloupec][kandidat] = true;
            }

            vyresene.Remove(bunka);
            nevyresene.Add(bunka);
        }

        public void ZvolKandidata(Bunka bunka, int kandidat)
        {
            HashSet<Bunka> zmeneneBunky = new HashSet<Bunka>();
            // Vybrat prvního kandidáta
            sudokuResene[bunka.Radek, bunka.Sloupec] = kandidat;
            omezeniBunek[bunka.Radek, bunka.Sloupec][kandidat] = false;

            omezeniRadku[bunka.Radek][kandidat] = true;
            omezeniSloupce[bunka.Sloupec][kandidat] = true;
            omezeniRegionu[bunka.Radek / 3, bunka.Sloupec / 3][kandidat] = true;


            // Odeberu kandidáty z polí souvisejici se zmenenou bunkou
            for (int i = 0; i < 9; i++)
            {
                if (sudokuResene[bunka.Radek, i] == 0)
                {
                    // radek
                    if (omezeniBunek[bunka.Radek, i][kandidat] == true)
                    {
                        omezeniBunek[bunka.Radek, i][kandidat] = false;

                        zmeneneBunky.Add(new Bunka(bunka.Radek, i));
                    }
                }

                if (sudokuResene[i, bunka.Sloupec] == 0)
                {
                    // sloupec
                    if (omezeniBunek[i, bunka.Sloupec][kandidat] == true)
                    {
                        omezeniBunek[i, bunka.Sloupec][kandidat] = false;

                        zmeneneBunky.Add(new Bunka(i, bunka.Sloupec));
                    }
                }
            }

            // region
            int pocatek_radkuR = bunka.Radek / 3 * 3;
            int pocatek_sloupceR = bunka.Sloupec / 3 * 3;
            for (int radek = pocatek_radkuR; radek < pocatek_radkuR + 3; radek++)
            {
                for (int sloupec = pocatek_sloupceR; sloupec < pocatek_sloupceR + 3; sloupec++)
                {
                    // only change unsolved cells containing the candidate
                    if (sudokuResene[radek, sloupec] == 0)
                    {
                        if (omezeniBunek[radek, sloupec][kandidat] == true)
                        {
                            //remove the candidate
                            omezeniBunek[radek, sloupec][kandidat] = false;

                            //update changed cells (for backtracking)
                            zmeneneBunky.Add(new Bunka(radek, sloupec));
                        }
                    }
                }
            }

            nevyresene.Remove(bunka);
            vyresene.Add(bunka);
            zmenene.Push(zmeneneBunky);
        }

        public bool VyresRekurzivne(Bunka dalsiBunka)   //dalsiBunka je bunka s minimem vhodnych kandidatu
        {
            //Vyřešeno
            if (dalsiBunka.Equals(new Bunka(-1, -1)))
            {
                Console.WriteLine("Sudoku is solved, Huray!");
                return true;
            }

            foreach (int kandidat in omezeniBunek[dalsiBunka.Radek, dalsiBunka.Sloupec])
            {
                ZvolKandidata(dalsiBunka, kandidat);

                //Backtracking
                if (VyresRekurzivne(DalsiBunka()) == false)
                {
                    OdeberKandidata(dalsiBunka, kandidat);
                    continue;
                }
                //Vyřešeno dříve
                else 
                    return true;
            }

            //Nemůže pokračovat dále, postup neodpovídá řešení
            return false;
        }

        public bool Solve()
        {
            return VyresRekurzivne(DalsiBunka());
        }

        public void VykresliSudoku()
        {
            for (int r = 0; r < 9; r++)
            {
                for (int s = 0; s < 9; s++)
                {
                    Console.Write("[{0}]", sudokuResene[r, s]);
                    if (s == 2 || s == 5)
                        Console.Write(" ");
                }
                Console.WriteLine();
                if (r == 2 || r == 5)
                    Console.WriteLine();
            }
        }

        public void VytiskniKandidaty()
        {
            for (int r = 0; r < 9; r++)
            {
                Console.Write("[{0}]: ", r);
                for (int s = 0; s < 9; s++)
                {
                    Console.Write("{0,-9}", omezeniBunek[r, s]);
                }
                Console.WriteLine();
            }
        }
    }
}
