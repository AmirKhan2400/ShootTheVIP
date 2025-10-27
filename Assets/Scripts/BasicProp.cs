using UnityEngine;

public class BasicProp : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth;
    [SerializeField] private MeshRenderer mesh;

    private Color fullHealthColor = Color.white;
    private Color zeroHealthColor = Color.red;

    private float health;

    private void Start()
    {
        health = maxHealth;
        fullHealthColor = mesh.material.color;
    }

    public void Damage(float DamageValue)
    {
        health = Mathf.Clamp(health - DamageValue, 0, maxHealth);

        if (health == 0)
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 1f);
        else
            UpdateColor();
    }

    void UpdateColor()
    {
        float healthPercent = health / maxHealth;
        Color newColor = Color.Lerp(zeroHealthColor, fullHealthColor, healthPercent);

        // if multiple materials:
        foreach (var mat in mesh.materials)
            mat.color = newColor;
    }
}