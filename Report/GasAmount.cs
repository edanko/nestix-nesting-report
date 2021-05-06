using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report
{
    public class GasAmount
    {
        public double Oxygen { get; private set; }
        public double Propan { get; private set; }
        public double Nitrogen { get; private set; }
        public double LaserMix { get; private set; }

        public static GasAmount GetGas(string machine, string quality, double weight)
        {
            const double plasmaOxygen = 5.56; // plasma - oxygen
            const double gasOxygen = 6.31; // gas - oxygen
            const int gasPropan = 2; // gas - propan
            const double laserNitrogenMetal = 0.16365879037204; // laser, n2, metal
            const double laserMixMetal = 0.000502461198510648; // laser - mix per kg, metal
            const double laserNitrogenPlywood = 22.8; // laser - n2, kg per list, plywood
            const double laserMixPlywood = 0.07; // laser - mix, m3, plywood

            var g = new GasAmount();

            switch (machine)
            {
                case "PlasmaBevelOmniMatL8000":
                    g.Oxygen = plasmaOxygen * weight / 1000.0;
                    break;
                case "GasBevelOmniMatL8000":
                case "GasOmniMatL7000":
                {
                    g.Oxygen = gasOxygen * weight / 1000.0;
                    g.Propan = gasPropan * weight / 1000.0;
                    break;
                }
                case "LaserMat4200":
                    if (quality == "PLYWOOD")
                    {
                        g.Nitrogen = laserNitrogenPlywood * 1.0;
                        g.LaserMix = laserMixPlywood * 1.0;
                    }
                    else
                    {
                        g.Nitrogen = laserNitrogenMetal * weight;
                        g.LaserMix = laserMixMetal * weight;
                    }
                    break;
            }
            return g;
        }
    }
}
