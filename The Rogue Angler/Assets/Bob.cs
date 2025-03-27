using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bob : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 2;
    private float gravity;
    private float xAxis, yAxis;
    [HideInInspector] public Rigidbody2D rb;
    private LineRenderer lineRenderer;
    private List<Vector3> points = new List<Vector3>();
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        gravity = rb.gravityScale;

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 0;
        // Criando um material padrão direto no código
        Material lineMaterial = new Material(Shader.Find("Unlit/Color"));
        lineMaterial.color = Color.white;
        lineRenderer.material = lineMaterial;
        lineRenderer.textureMode = LineTextureMode.Tile;
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();
        Move();
        DrawTrail();
        DestroyBob();
    }

    void DestroyBob()
    {
        if (Input.GetButtonDown("Interact"))
        {
            Destroy(gameObject); // Destroi o objeto Bob
            PlayerController.Instance.pState.bobing = false; // Marca que o Bob não está mais sendo usado
            GameManager.Instance.gameIsPaused = false;
        }
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
    }

    private void Move()
    {
        rb.velocity = new Vector2(walkSpeed * xAxis, walkSpeed * yAxis);
    }

    private void DrawTrail()
    {
        Vector3 currentPosition = transform.position;

        // Adiciona ponto se a posição mudou significativamente
        if (points.Count == 0 || Vector3.Distance(points[points.Count - 1], currentPosition) > 0.1f)
        {
            points.Add(currentPosition);
            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.ToArray());
        }
    }
}
