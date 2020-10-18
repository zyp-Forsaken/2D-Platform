using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

///<summary>
///
///<summary>
public class PlayerController : MonoBehaviour
{   /// <summary>
    /// 定义变量
    /// </summary>
    [Header("跑步")]
    public float RunSpeed = 5;

    [Header("跳跃")]
    public float JumpSpeed = 5;
    public bool IsJumping = false;//玩家是否正在跳跃
    float PlayerHasYAxisSpeed;//玩家y轴速度
    bool Rising;//玩家是否正在上升
    bool Falling;//玩家是否正在下降
    public float FallMultiplier = 4;//玩家下降的最大速度
    public float LowJumpMultiplier = 2.5f;//玩家上升的最大速度

    [Header("落地判定")]
    public bool IsOnGround;

    public float VelocityX;//我也不知道有啥用
    public float AccelerateTime = 0.09f;//从静止到全速运动所需要的时间
    public float DecelerateTime = 0.09f;//从全速运动到静止所需要的时间

    private Rigidbody2D PlayerRigidbody2D;//角色的刚体
    private Animator PlayerAnimator;//角色的动画编辑器
    private Collider2D PlayerFeet;//角色脚上的碰撞器
    private void Start()
    {
        PlayerRigidbody2D = GetComponent<Rigidbody2D>();//初始化
        PlayerAnimator = GetComponent<Animator>();
        PlayerFeet = GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        Anime();
    }
    private void FixedUpdate()
    {
        Jump();
        Run();
        Flip();
    }
    /// <summary>
    /// 跳跃
    /// </summary>
    private void Jump()
    {
        //落地刷新跳跃
        IsOnGround = OnGround();
        if (Input.GetButton("Jump") && IsJumping == false)
        {
            PlayerRigidbody2D.velocity = new Vector2(PlayerRigidbody2D.velocity.x, JumpSpeed);
            IsJumping = true;
        }
        else if (IsOnGround=true && Input.GetAxisRaw("Jump") == 0)
        {
            IsJumping = false;
        }
        //通过按键时间长短调整滞空时间
        if (PlayerRigidbody2D.velocity.y < 0)//当玩家下坠的时候
        {
            PlayerRigidbody2D.velocity += Vector2.up * Physics.gravity.y * (FallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (PlayerRigidbody2D.velocity.y > 0 && Input.GetAxis("Jump") != 1)//当玩家上升且松开跳跃键的时候
        {
            PlayerRigidbody2D.velocity += Vector2.up * Physics.gravity.y * (LowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }
    /// <summary>
    /// 改变人物朝向
    /// </summary>
    private void Flip()
    {
        if (PlayerRigidbody2D.velocity.x > Mathf.Epsilon)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (PlayerRigidbody2D.velocity.x < -Mathf.Epsilon)
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
    }
    /// <summary>
    /// 没事走两步
    /// </summary>
    private void Run()
    {
        //移动逻辑
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            PlayerRigidbody2D.velocity = new Vector2(Mathf.SmoothDamp(PlayerRigidbody2D.velocity.x, RunSpeed * Time.fixedDeltaTime * 60, ref VelocityX, AccelerateTime), PlayerRigidbody2D.velocity.y);
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            PlayerRigidbody2D.velocity = new Vector2(Mathf.SmoothDamp(PlayerRigidbody2D.velocity.x, RunSpeed * Time.fixedDeltaTime * 60 * -1, ref VelocityX, AccelerateTime), PlayerRigidbody2D.velocity.y);
        }
        else //if (Input.GetAxisRaw("Horizontal") == 0)
        {
            PlayerRigidbody2D.velocity = new Vector2(Mathf.SmoothDamp(PlayerRigidbody2D.velocity.x, 0, ref VelocityX, DecelerateTime), PlayerRigidbody2D.velocity.y);
        }
    }
    /// <summary>
    /// 动画的切换
    /// </summary>
    private void Anime()
    {
        //跑步动画
        //判断玩家x轴速度是否大于0
        bool playerHasXAxisSpeed = (Math.Abs(PlayerRigidbody2D.velocity.x) > 0) || (Math.Abs(PlayerRigidbody2D.velocity.y) < 0);
        //把判断的值赋值给animator组件
        PlayerAnimator.SetBool("Run", playerHasXAxisSpeed);
        //跳跃动画        
        PlayerHasYAxisSpeed = PlayerRigidbody2D.velocity.y;
        if (PlayerHasYAxisSpeed < Mathf.Epsilon && PlayerHasYAxisSpeed > -Mathf.Epsilon)//判断y轴速度是否等于零
        {
            Rising = false;
            Falling = false;
        }
        else if (PlayerHasYAxisSpeed > Mathf.Epsilon)
        {
            Falling = false;
            Rising = true;
        }
        else //if (PlayerHasYAxisSpeed < Mathf.Epsilon)
        {
            Rising = false;
            Falling = true;
        }
        PlayerAnimator.SetBool("Rise", Rising);
        PlayerAnimator.SetBool("Fall", Falling);

    }
    /// <summary>
    /// 是否落地
    /// </summary>
    /// <returns></returns>
    private bool OnGround()
    {
        IsOnGround = PlayerFeet.IsTouchingLayers(LayerMask.GetMask("Ground"));
        return IsOnGround;
    }
}


