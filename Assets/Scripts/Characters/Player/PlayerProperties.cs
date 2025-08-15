using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using static UnityEngine.UISystemProfilerApi;

public partial class Player
{
    [Header("Player Properties")]

    #region Player Data

    [Header(" └─ Data")]
    protected new string name;

    public string _name
    {
        get { return name; }
        set { name = value; }
    }

    #endregion Player Data

    #region Player Skills

    [Header(" └─ Skills")]
    protected List<string> skills = new List<string>();

    public List<string> Skills
    {
        get { return skills; }
        set { skills = value; }
    }

    #endregion Player Skills

    #region Player Movement

    [Header(" └─ Movement")]
    public Vector2 moveInput;

    protected CharacterController controller;

    protected Vector3 move;
    protected Quaternion rotation;
    public float rotateSpeed = 10.0f;
    public float _moveSpeed = 10.0f;

    public float moveSpeed
    {
        get { return _moveSpeed; }
        set { _moveSpeed = value; }
    }

    [Header(" └─ Jump")]
    public float jumpForce = 7f;

    private float verticalVelocity;

    public float gravity = -20f;
    private int jumpCount = 0;
    public int maxJumps = 2;
    private bool isGliding = false;
    public float glideGravity = -3f;

    #endregion Player Movement

    #region Player Health

    [Header(" └─ Health")]
    protected int life;

    public int _life
    {
        get { return life; }
        set { life = value; }
    }

    protected int maxLife;

    public int MaxLife
    {
        get { return maxLife; }
        set { maxLife = value; }
    }

    #endregion Player Health

    #region Unity Tools

    [Header(" └─ Camera")]
    public GameObject mainCamera;

    private CameraController cameraController;

    #endregion Unity Tools
}