using System;
using System.Security.Cryptography.X509Certificates;

namespace Core.Specifications;
//On a crée cette classe afin de permettre de faire des filtrage quand on a recuperer des
// products depuis la bd
// Au lieu de mettre GetProduct(string? brand, string? type, string? sort), on va créer un objet
public class ProductSpecParams
{
    //Ici, on fixe une taille maximale de page à 50.
    //Ça sert de sécurité pour éviter que quelqu’un ne demande 1000 produits d’un coup dans l’API.
    private const int MaxPageSize = 50;
    //PageIndex = numéro de la page que l’utilisateur demande.
    //Par défaut = 1 (première page).
    public int PageIndex { get; set; } = 1;

    //_pageSize = taille par défaut de la page (ici 6).
    private int _pageSize = 6;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }


    private List<string> _brands = [];
    public List<string> Brands
    {
        // set → reçoit la donnée de l’application et la prépare
        // get → renvoie la donnée prête à l’emploi

        get => _brands; //types=boards,gloves 
        set
        {
            _brands = value.SelectMany(b => b.Split(',', StringSplitOptions.RemoveEmptyEntries)).ToList();
        }
    }

    private List<string> _types = [];
    public List<string> Types
    {
        get => _types;
        set
        {
            _types = value.SelectMany(b => b.Split(',', StringSplitOptions.RemoveEmptyEntries)).ToList();
        }
    }

    public string? Sort { get; set; }

    private string? _search;
    public string Search
    {
        get => _search ?? "";
        set => _search = value.ToLower();
    }

}
