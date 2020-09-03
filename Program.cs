using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;
using GAF;
using GAF.Operators;

namespace geogenetic
{
    class Program
    {
        static DateTime t1, t2;
        static int npontos;
        static double maxdim;
        static double rp;
        static double[,] mpontos;
        static double[] vrange;
        static string[,] mevolution;
        
        
        struct parameters
        {
            public int ga_gen;
            public int ga_pop;
            public double ga_cros;
            public double ga_mut;
            public double ga_elit;
            public double ga_ns;
            public int sa_to;
            public int sa_ite;
            public double geo_amin;
            public double geo_amax;
            public double geo_tmin;
            public double geo_tmax;
            public double geo_bwmin;
            public double geo_bwmax;
            public double geo_lmin;
            public double geo_lmax;
            public int geo_nlmin;
            public int geo_nlmax;
        }
        static parameters p;

        static tag tagg = new tag();
        List<string> lst = new List<string>();

        static void Main(string[] args)
        {
            
            input();
            ga();
            output();
           
        }

      
        static void input()
        {

           
            #region initial

            //Penalty Function 2 : 10%
            rp = 0.1;
            t1 = DateTime.Now;
            Console.WriteLine("GEOGENETIC");
            Console.WriteLine("Geoestatistical(2D) + Genetic Algorithm");
            Console.WriteLine("---------------------------------------");
            Console.WriteLine("Creator:Cira Souza Pitombo");
            Console.WriteLine("University of São Paulo, Brazil");
            Console.WriteLine("cirapitombo@usp.br");
            Console.WriteLine("Developer:Luis Henrique Magalhães Costa");
            Console.WriteLine("Vale do Acaraú State University,Sobral,Brazil");
            Console.WriteLine("luis_costa@uvanet.br");
            Console.WriteLine("---------------------------------------");
            string pathexe = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string pathdir = Path.GetDirectoryName(pathexe);
            string[] vfiles = Directory.GetFiles(pathdir);
            List<string> lfiles = new List<string>();
            foreach (string stf in vfiles)
                if (stf.IndexOf(".txt") != -1)
                    lfiles.Add(Path.GetFileName(stf));
            Console.WriteLine("Choose the input file (ex: 1,2,3...)");
            int k = 0;
            foreach (string stf in lfiles)
            {
                k++;
                Console.WriteLine(k.ToString() + "-" + stf);
            }
            Console.WriteLine("-------------------");
            int intfile = Convert.ToInt32(Console.ReadLine());
            string st = lfiles[intfile - 1];
            string[] vst = System.IO.File.ReadAllLines(st);
            string[] vst2 = new string[vst.Length];
            List<string> linp = new List<string>();
            for (int i = 0; i < vst.Count(); i++)
            {
                string st1 = vst[i];
                string st2 = st1.Replace("\t", " ");
                string st3 = Regex.Replace(st2, " {2,}", " ");
                string st4 = st3.Trim();

                if (st4.Length > 0) if (st4.Substring(0, 1) != ";") linp.Add(st4);
            }
            #endregion           
            #region GA
            int count = 0;
            
            foreach (string line in linp)
                if (line.IndexOf("[GA PARAMETERS]") != -1) count++;
            if (count == 0)
            {
                Console.WriteLine("[GA PARAMETERS] not exist. Check your input file!");
                goto end;
            }
            int k1 = 0;
            k = -1;
            foreach (string linha in linp)
            {
                k++;
                if (linha.IndexOf("[GA PARAMETERS]")!=-1)
                {
                    k1 = k;
                    break;
                }
            }
            k1++;
            p.ga_gen = Convert.ToInt32(linp[k1]);
            k1++;
            p.ga_pop = Convert.ToInt32(linp[k1]);
            k1++;
            p.ga_cros = Convert.ToDouble(linp[k1]) / 100;
            k1++;
            p.ga_mut = Convert.ToDouble(linp[k1]) / 100;
            k1++;
            p.ga_elit = Convert.ToDouble(linp[k1]);
            k1++;
            p.ga_ns = Convert.ToDouble(linp[k1]) / 100;

            mevolution = new string[p.ga_gen + 1,16];
            #endregion

            #region SA
            count = 0;

            foreach (string line in linp)
                if (line.IndexOf("[SA PARAMETERS]") != -1) count++;
            if (count == 0)
            {
                Console.WriteLine("[SA PARAMETERS] not exist. Check your input file!");
                goto end;
            }
            k1 = 0;
            k = -1;
            foreach (string linha in linp)
            {
                k++;
                if (linha.IndexOf("[SA PARAMETERS]") != -1)
                {
                    k1 = k;
                    break;
                }
            }
            k1++;
            p.sa_ite = Convert.ToInt32(linp[k1]);
            k1++;
            p.sa_to = Convert.ToInt32(linp[k1]);
            k1++;
            #endregion

            #region GEO
            count = 0;

            foreach (string line in linp)
                if (line.IndexOf("[GEOESTATISTICAL PARAMETERS]") != -1) count++;
            if (count == 0)
            {
                Console.WriteLine("[GEOESTATISTICAL PARAMETERS] not exist. Check your input file!");
                goto end;
            }
            k1 = 0;
            k = -1;
            foreach (string linha in linp)
            {
                k++;
                if (linha.IndexOf("[GEOESTATISTICAL PARAMETERS]") != -1)
                {
                    k1 = k;
                    break;
                }
            }
            k1++;
            string[] vst1 = linp[k1].Split('/');
            p.geo_amax = Convert.ToDouble(vst1[1]);
            p.geo_amin = Convert.ToDouble(vst1[0]);
            k1++;
            vst1 = linp[k1].Split('/');
            p.geo_tmax = Convert.ToDouble(vst1[1]);
            p.geo_tmin = Convert.ToDouble(vst1[0]);
            k1++;
            vst1 = linp[k1].Split('/');
            p.geo_bwmax = Convert.ToDouble(vst1[1]);
            p.geo_bwmin = Convert.ToDouble(vst1[0]);
            k1++;
            vst1 = linp[k1].Split('/');
            p.geo_lmax = Convert.ToDouble(vst1[1]);
            p.geo_lmin = Convert.ToDouble(vst1[0]);
            k1++;
            vst1 = linp[k1].Split('/');
            p.geo_nlmax = Convert.ToInt32(vst1[1]);
            p.geo_nlmin = Convert.ToInt32(vst1[0]);

            #endregion

            #region DATA
            count = 0;

            foreach (string line in linp)
                if (line.IndexOf("[DATA]") != -1) count++;
            if (count == 0)
            {
                Console.WriteLine("[DATA] not exist. Check your input file!");
                goto end;
            }
            k1 = 0;
            k = -1;
            foreach (string linha in linp)
            {
                k++;
                if (linha.IndexOf("[DATA]") != -1)
                {
                    k1 = k;
                    break;
                }
            }
            k1++;
            List<data> ldata = new List<data>();
            for (int i = k1; i <= linp.Count - 1; i++)
            {
                string[] vst3 = linp[i].Split(' ');
                data data = new data();
                data.x = Convert.ToDouble(vst3[0].Replace('.',','));
                data.y = Convert.ToDouble(vst3[1].Replace('.', ','));
                data.z = Convert.ToDouble(vst3[2].Replace('.', ','));
                ldata.Add(data);
            }
            double maxy = 0;
            double miny = 1000000000;
            double maxx = 0;
            double minx = 1000000000;
            foreach (data d in ldata)
            {
                if (d.x > maxx) maxx = d.x;
                if (d.y > maxy) maxy = d.y;
                if (d.x < minx) minx = d.x;
                if (d.y < miny) miny = d.y;
            }
            double max1 = System.Math.Abs(maxx - minx);
            double max2 = System.Math.Abs(maxy - miny);
            if (max1 > max2) maxdim = max1;
            else maxdim = max2;
                
        
            Console.WriteLine("----------------------");
            Console.WriteLine("Max Distance");
            Console.WriteLine("Axis x: {0}", System.Math.Abs((maxx - minx)));
            Console.WriteLine("Axis y: {0}", System.Math.Abs((maxy - miny)));
            Console.WriteLine("----------------------");

            int np = ldata.Count;
            npontos = np;
            mpontos = new double[np + 1, 6];
            mpontos[0, 0] = np;
            int m = 0;
            foreach (data d in ldata)
            {
                m++;
                mpontos[m, 1] = d.x;
                mpontos[m, 2] = d.y;
                mpontos[m, 3] = d.z;
            }

            string[] vinput = new string[npontos + 5];
            vinput[0] = "input.dat";
            vinput[1] = "3";
            vinput[2] = "X";
            vinput[3] = "Y";
            vinput[4] = "Z";
            for (int i = 1; i <= npontos; i++)
            {
                string s1 = mpontos[i, 1].ToString("F4").Replace(",", ".");
                string s2 = mpontos[i, 2].ToString("F4").Replace(",", ".");
                string s3 = mpontos[i, 3].ToString("F4").Replace(",", ".");
                string linha = s1 + " " + s2 + " " + s3;
                vinput[i + 4] = linha;
            }
            File.WriteAllLines("input.dat", vinput);
        #endregion

        end:;
        }

        static void output()
        {
           
            
            List<string> lst = new List<string>();
            lst.Add("Objective Function: " + tagg.fobj.ToString());
            lst.Add("Angle: " + tagg.angle.ToString());
            lst.Add("Tolerance: " + tagg.tol.ToString());
            lst.Add("BandWidth: " + tagg.bwh.ToString());
            lst.Add("Lag: " + tagg.lsd.ToString());
            lst.Add("Nlag: " + tagg.nl.ToString());
            lst.Add("Nugget: " + tagg.C0.ToString());
            lst.Add("Sill: " + tagg.C.ToString());
            lst.Add("Range: " + tagg.a.ToString());
            lst.Add("Curve: " + tagg.curve.ToString());
            lst.Add("Loss Function: " + tagg.floss.ToString());
            
            lst.Add("------------------------------------");
            lst.Add("Generation - Objective Function - Angle - Tolerance - Bandwith - Lag - Nlag - Nugget - Sill - Range - N/S - Curve - Loss Function - variogram x - variogram y");
            lst.Add("------------------------------------");
            
            for (int i = 1; i <= p.ga_gen; i++)
            {
                string st = "";
                for (int j = 1; j <= 15; j++) st = st + mevolution[i, j] + " ";
                lst.Add(st);
            }
            string[] vst = lst.ToArray();
            System.IO.File.WriteAllLines("output.txt", vst);
            Console.WriteLine("--------------------------");
            Console.WriteLine("Check the file output.txt");
            Console.WriteLine("Hit enter do finish");
            Console.ReadKey();

        }

        #region GENETIC_ALGORITHM
        static void ga()
        {
            vrange = new double[6];
            vrange[1] = GAF.Math.GetRangeConstant(p.geo_amax - p.geo_amin, 10);
            vrange[2] = GAF.Math.GetRangeConstant(p.geo_tmax - p.geo_tmin, 10);
            vrange[3] = GAF.Math.GetRangeConstant(p.geo_bwmax - p.geo_bwmin, 10);
            vrange[4] = GAF.Math.GetRangeConstant(p.geo_lmax - p.geo_lmin, 10);
            vrange[5] = GAF.Math.GetRangeConstant(p.geo_nlmax - p.geo_nlmin, 10);
            
            var pop = new Population(p.ga_pop, 50);
            var elite = new Elite(Convert.ToInt32(p.ga_elit));
            var crossover = new Crossover(p.ga_cros);
            var mutacao = new BinaryMutate(p.ga_mut);
            var ag = new GeneticAlgorithm(pop, fapt2d);
            ag.Operators.Add(elite);
            ag.Operators.Add(crossover);
            ag.Operators.Add(mutacao);
            ag.OnGenerationComplete += geracao2d;
            Console.WriteLine("----------------------------------");
            Console.WriteLine("Starting...");
            Console.WriteLine("----------------------------------");
            ag.Run(fim2d);
        }

        static double fapt2d(Chromosome sol)
        {
            string[] vcurve = { "shp", "exp", "gaus" };
            double[] vsol = bintoreal2D(sol);
            double[,] mvar = variograma2D(vsol);
            int nvar = Convert.ToInt32(mvar[0, 0]);
            double[] vvarx = new double[nvar + 1];
            double[] vvary = new double[nvar + 1];
            double r = 0;
            for (int i = 1; i <= nvar; i++)
            {
                vvarx[i] = mvar[i, 1];
                vvary[i] = mvar[i, 2];
            }
            double soma = 0;
            bool vario = true;
            double somavar = vvary.Sum();
            if (somavar < 0.1) vario = false;
            int cont = 0;
            for (int i = 2; i <= nvar; i++) if (vvary[i] == 0) cont++;
            if (cont > 0) vario = false;
            if (vario == false)
            {
                r = 0;
                goto fim;
            }
            double[] param = sa.simulated(vvarx, vvary);
            int tipo = Convert.ToInt32(param[0]);
            double floss = param[1];
            double C0 = param[2];
            double C = param[3];
            double a = param[4];
            //--------penalty 1
            double fp1 = 1; 
            if (C0 / C > p.ga_ns) fp1 = System.Math.Exp(2 * p.ga_ns);
            //--------penalty 2 
            double fp2 = 1;
            double R=System.Math.Abs((2*vsol[4]*(int)vsol[5]-maxdim)/maxdim);
            if (R > rp) fp2 = 2 * System.Math.Log(30 * R);
            //--------penalty 3 
            //double fp3 = System.Math.Sqrt(floss / nvar);
            //------------------------
            for (int i = 1; i <= npontos; i++)
            {
                double x = mpontos[i, 1];
                double y = mpontos[i, 2];
                double z = mpontos[i, 3];
                double zest = krigagem.krigagem2D(x, y, mpontos, tipo, C0, C, a);
                soma = soma + (z - zest) * (z - zest);
                //soma2 = soma2 + (z - mediaobs)* (z - mediaobs);
            }
            soma = soma / npontos;

            soma = soma * fp1*fp2;

            //string st = vsol[1].ToString("F2")+"/"+ vsol[2].ToString("F2") + "/" + vsol[3].ToString("F2") + "/" + vsol[4].ToString("F2") + "/" + Convert.ToInt32(vsol[5]).ToString() + "/" +C0.ToString("F2")+"/"+C.ToString("F2")+ "/" + a.ToString("F2")+ "/"+vcurve[tipo-1]+"/" +floss.ToString("F2")+"/"+soma.ToString("F2"); 
            //string st = soma.ToString("F4") + "/" + tipo.ToString() + "/" + C0.ToString("F4") + "/" + C.ToString("F4") + "/" + a.ToString("F4");
            tag Tag = new tag();
            Tag.ns = C0 / C;
            Tag.angle = vsol[1]; Tag.tol = vsol[2]; Tag.bwh = vsol[3]; Tag.lsd = vsol[4];
            Tag.nl = (int)vsol[5]; Tag.C0 = C0; Tag.C = C; Tag.a = a;
            Tag.curve = vcurve[tipo - 1]; Tag.floss = floss; Tag.fobj = soma;
            Tag.vx = vvarx; Tag.vy = vvary; Tag.curvetipo = tipo;
            sol.Tag = Tag;

           

            double gama = 0.1;
            double aux1 = 1 / (1 + System.Math.Exp(-gama * soma));
            r = 2 * (1 - aux1);
            fim:
            if (r < 0) r = 0;
            //Console.WriteLine(r.ToString());
            return r;
        }

        static double[] bintoreal2D(Chromosome sol)
        {

            var x1 = Convert.ToInt32(sol.ToBinaryString(0, 10), 2);
            var x2 = Convert.ToInt32(sol.ToBinaryString(10, 10), 2);
            var x3 = Convert.ToInt32(sol.ToBinaryString(20, 10), 2);
            var x4 = Convert.ToInt32(sol.ToBinaryString(30, 10), 2);
            var x5 = Convert.ToInt32(sol.ToBinaryString(40, 10), 2);
            double[] vr = new double[6];
            vr[1] = (x1 * vrange[1]) + p.geo_amin;
            vr[2] = (x2 * vrange[2]) + p.geo_tmin;
            vr[3] = (x3 * vrange[3]) + p.geo_bwmin;
            vr[4] = (x4 * vrange[4]) + p.geo_lmin;
            vr[5] = (x5 * vrange[5]) + p.geo_nlmin;
            return vr;
        }
        static void geracao2d(object sender, GaEventArgs e)
        {
            var sol = e.Population.GetTop(1)[0];
            tagg = sol.Tag as tag;
            if (tagg != null)
            {
                //  tagg2 = sol2.Tag as tag;
                tagg.generation = e.Generation;
                mevolution[e.Generation,1] = tagg.generation.ToString("F4");
                mevolution[e.Generation, 2] = tagg.fobj.ToString("F4");
                mevolution[e.Generation, 3] = tagg.angle.ToString("F4");
                mevolution[e.Generation, 4] = tagg.tol.ToString("F4");
                mevolution[e.Generation, 5] = tagg.bwh.ToString("F4");
                mevolution[e.Generation, 6] = tagg.lsd.ToString("F4");
                mevolution[e.Generation, 7] = tagg.nl.ToString();
                mevolution[e.Generation, 8] = tagg.C0.ToString("F4");
                mevolution[e.Generation, 9] = tagg.C.ToString("F4");
                mevolution[e.Generation, 10] = tagg.a.ToString("F4");
                mevolution[e.Generation, 11] = tagg.ns.ToString("F4");
                mevolution[e.Generation, 12] = tagg.curve.ToString();
                mevolution[e.Generation, 13] = tagg.floss.ToString("F4");
                string stx = "";
                string sty = "";
                int lvar = tagg.vx.Length;
                for (int i = 1; i <= lvar - 1; i++)
                {
                    string sep = "";
                    if (i < lvar - 1) sep = "-"; else sep = " ";
                    stx += tagg.vx[i].ToString("F4")+sep;
                    sty += tagg.vy[i].ToString("F4") + sep;
                }
                mevolution[e.Generation, 14] = stx;
                mevolution[e.Generation, 15] = sty;
                t2 = DateTime.Now;
                var duracao = (t2 - t1).Minutes;

                Console.WriteLine("Generation: {0}", tagg.generation);
                Console.WriteLine("Objective Function: {0}", tagg.fobj);
                Console.WriteLine("Angle - Tolerance - BandWidth: {0} - {1} - {2}", tagg.angle, tagg.tol, tagg.bwh);
                Console.WriteLine("Lag - Nlag: {0} - {1}", tagg.lsd, tagg.nl);
                Console.WriteLine("Nugget - Sill - Range: {0} - {1} - {2}", tagg.C0, tagg.C, tagg.a);
                Console.WriteLine("Nugget/Sill: {0}", tagg.ns);
                Console.WriteLine("Curve: {0}", tagg.curve);
                Console.WriteLine("Loss Function: {0}", tagg.floss);
                Console.WriteLine("Total Time (min): {0}", duracao);
                Console.WriteLine("--------------------------");

                
            }
            else
            {
                Console.WriteLine("Generation: {0}", e.Generation);
                Console.WriteLine("Not Viable Solution");
            }

           



        }
        static bool fim2d(Population population, int currentGeneration, long currentEvaluation)
        {
            return currentGeneration >= Convert.ToInt32(p.ga_gen);
        }
        #endregion

        static double[,] variograma2D(double[] vsol)
        {
            // atualiza .par
            FileInfo info = new FileInfo("gamv2D.par");
            //while (fileinuse(info)) { }
            //string[] vfile = File.ReadAllLines("gamv2D.par");
            string[] vfile = new string[14];
            vfile[0] = "START OF PARAMETERS:";
            vfile[1] = "input.dat";
            vfile[2] = "1 2 0";
            vfile[3] = "1   3";
            vfile[4] = "-1.0e21  1.0e21";
            vfile[5] = "gamv2D.out";

            vfile[6] = Convert.ToInt16(vsol[5]).ToString();
            vfile[7] = vsol[4].ToString("F2").Replace(",", "."); ;
            vfile[8] = (vsol[4] / 2).ToString("F2").Replace(",", ".");
            vfile[9] = "1";
            string st1 = vsol[1].ToString("F2").Replace(",", ".");
            string st2 = vsol[2].ToString("F2").Replace(",", ".");
            string st3 = vsol[3].ToString("F2").Replace(",", ".");
            string linha = st1 + " " + st2 + " " + st3 + " 0.0 10.0 10.0";
            vfile[10] = linha;
            vfile[11] = "0";
            vfile[12] = "1";
            vfile[13] = "1   1   1";
            while (fileinuse(info)) { }
            File.WriteAllLines("gamv2D.par", vfile);


            //roda gslib
            while (fileinuse(info)) { }
            var proc = new Process();
            proc.StartInfo.FileName = "gamv.exe";
            proc.StartInfo.Arguments = "gamv2D.par";
            //proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.Start();
            proc.WaitForExit();
            int cod = proc.ExitCode;
            proc.Close();

            //capta variograma
            string[] vst = File.ReadAllLines("gamv2D.out");
            int nvar = vst.Count() - 1;
            double[,] mvar = new double[nvar + 1, 3];
            mvar[0, 0] = nvar;
            for (int i = 1; i <= nvar; i++)
            {
                string st4 = vst[i];
                string[] vst2 = st4.Split(' ');
                int count = 0;
                double db1 = 0; double db2 = 0;
                foreach (string s in vst2)
                {
                    if (s != "") count++;
                    if (count == 2)
                    {
                        db1 = Convert.ToDouble(s.Replace('.', ','));
                        count++;
                    }
                    if (count == 4) db2 = Convert.ToDouble(s.Replace('.', ','));
                    if (count == 4) break;
                }
                mvar[i, 1] = db1;
                mvar[i, 2] = db2;
            }
            return mvar;
        }

        static bool fileinuse(FileInfo file)
        {

            FileStream stream = null;
            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                Thread.Sleep(100);
                return true;
            }
            finally
            {
                if (stream != null) stream.Close();
            }
            return false;
        }
    }
}
