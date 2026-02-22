using UnityEngine;
using TMPro;
using DG.Tweening;

public class InventoryUI : MonoBehaviour
{
    [Header("Textos de cantidades (dentro del panel)")]
    [SerializeField] private TextMeshProUGUI cantidadPokeball;
    [SerializeField] private TextMeshProUGUI cantidadSuperball;
    [SerializeField] private TextMeshProUGUI cantidadUltraball;
    [SerializeField] private TextMeshProUGUI cantidadHonorball;

    [Header("Configuración DOTween")]
    [SerializeField] private float duracionApertura = 0.45f;
    [SerializeField] private Ease easeApertura = Ease.OutBack;
    [SerializeField] private float duracionCierre = 0.3f;
    [SerializeField] private Ease easeCierre = Ease.InBack;

    private DatabaseManager db;
    private int userId;

    private void Start()
    {
        db = FindObjectOfType<DatabaseManager>();
        if (db == null)
        {
            Debug.LogError("No se encontró DatabaseManager en la escena");
            return;
        }

        userId = PlayerPrefs.GetInt("CurrentUserID", -1);
        if (userId == -1)
        {
            Debug.LogError("No hay usuario logueado (CurrentUserID no encontrado)");
            return;
        }

        ActualizarCantidades();
    }

    // ---- MÉTODO GENERAL ----

    private void ModificarItem(int itemId, bool sumar)
    {
        if (sumar)
            db.AddItem(userId, itemId);
        else
            db.RestarItem(userId, itemId);

        ActualizarCantidades();
    }

    // ---- MÉTODOS PARA BOTONES ----

    public void AñadirPokeball()        => ModificarItem(1, true);
    public void DisminuirPokeball()     => ModificarItem(1, false);

    public void AñadirSuperball()       => ModificarItem(2, true);
    public void DisminuirSuperball()    => ModificarItem(2, false);

    public void AñadirUltraball()       => ModificarItem(3, true);
    public void DisminuirUltraball()    => ModificarItem(3, false);

    public void AñadirHonorball()       => ModificarItem(4, true);
    public void DisminuirHonorball()    => ModificarItem(4, false);

    // ---- ACTUALIZAR UI ----

    private void ActualizarCantidades()
    {
        ActualizarTexto(cantidadPokeball, 1);
        ActualizarTexto(cantidadSuperball, 2);
        ActualizarTexto(cantidadUltraball, 3);
        ActualizarTexto(cantidadHonorball, 4);
    }

    private void ActualizarTexto(TextMeshProUGUI texto, int itemId)
    {
        if (texto != null)
            texto.text = db.GetCantidad(userId, itemId).ToString();
    }
}