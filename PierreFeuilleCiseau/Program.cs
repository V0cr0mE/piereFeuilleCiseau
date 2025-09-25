using System;
using System.Collections.Generic;

enum Coup
{
    Pierre,
    Feuille,
    Ciseau
}

static class CoupExtensions
{
    public static string VersNomLisible(this Coup coup)
    {
        return coup switch
        {
            Coup.Pierre => "pierre",
            Coup.Feuille => "feuille",
            Coup.Ciseau => "ciseau",
            _ => throw new ArgumentOutOfRangeException(nameof(coup), coup, null)
        };
    }
}

class Program
{
    private static readonly Dictionary<string, Coup> _aliasVersCoup = new(StringComparer.OrdinalIgnoreCase)
    {
        ["p"] = Coup.Pierre,
        ["pi"] = Coup.Pierre,
        ["pierre"] = Coup.Pierre,
        ["f"] = Coup.Feuille,
        ["feuille"] = Coup.Feuille,
        ["c"] = Coup.Ciseau,
        ["ci"] = Coup.Ciseau,
        ["ciseau"] = Coup.Ciseau,
        ["ciseaux"] = Coup.Ciseau
    };

    private static readonly string _invite = "Choisissez votre coup : pierre (p), feuille (f), ciseau (c) ou q pour quitter";

    private static readonly Random _aleatoire = new();

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("Bienvenue dans Pierre-Feuille-Ciseau !");
        Console.WriteLine("Affrontez le robot en choisissant votre coup à chaque manche.");
        Console.WriteLine("Entrez q à tout moment pour quitter la partie.");

        int scoreHumain = 0;
        int scoreRobot = 0;
        int egalites = 0;

        while (true)
        {
            var coupHumain = DemanderCoupHumain();
            if (coupHumain is null)
            {
                break;
            }

            var coupRobot = GenererCoupRobot();

            Console.WriteLine($"Le robot joue {coupRobot.VersNomLisible()}.");

            var resultat = ComparerCoups(coupHumain.Value, coupRobot);

            switch (resultat)
            {
                case ResultatManche.Humain:
                    scoreHumain++;
                    Console.WriteLine("Vous gagnez cette manche ! 🎉");
                    break;
                case ResultatManche.Robot:
                    scoreRobot++;
                    Console.WriteLine("Le robot gagne cette manche. 🤖");
                    break;
                case ResultatManche.Egalite:
                    egalites++;
                    Console.WriteLine("Égalité parfaite !");
                    break;
            }

            Console.WriteLine($"Score => Humain: {scoreHumain} | Robot: {scoreRobot} | Égalités: {egalites}");
        }

        Console.WriteLine("Merci d'avoir joué. À bientôt !");
    }

    private static Coup? DemanderCoupHumain()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine(_invite + " :");
            Console.Write("> ");

            var saisie = Console.ReadLine();
            if (saisie is null)
            {
                return null;
            }

            saisie = saisie.Trim();

            if (string.Equals(saisie, "q", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(saisie, "quit", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(saisie, "quitter", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (_aliasVersCoup.TryGetValue(saisie, out var coup))
            {
                return coup;
            }

            Console.WriteLine("Saisie non reconnue. Essayez encore en tapant pierre, feuille ou ciseau.");
        }
    }

    private static Coup GenererCoupRobot()
    {
        var valeur = _aleatoire.Next(0, 3);
        return (Coup)valeur;
    }

    private static ResultatManche ComparerCoups(Coup humain, Coup robot)
    {
        if (humain == robot)
        {
            return ResultatManche.Egalite;
        }

        return (humain, robot) switch
        {
            (Coup.Pierre, Coup.Ciseau) => ResultatManche.Humain,
            (Coup.Feuille, Coup.Pierre) => ResultatManche.Humain,
            (Coup.Ciseau, Coup.Feuille) => ResultatManche.Humain,
            _ => ResultatManche.Robot
        };
    }

    private enum ResultatManche
    {
        Humain,
        Robot,
        Egalite
    }
}
