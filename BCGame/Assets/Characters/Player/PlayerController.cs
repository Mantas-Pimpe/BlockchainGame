using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;

    public ContactFilter2D movementFilter;

    Vector2 movementInput;
    Rigidbody2D rb;
    Animator animator;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    public List<TokenEntity> items;

    public GameObject marketCanvas;
    public GameObject inventoryCanvas;

    public GameObject marketContent;
    public GameObject inventoryContent;


    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        items = new List<TokenEntity>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if(marketCanvas.activeSelf){
                marketCanvas.SetActive(false);
            } else {
                marketCanvas.SetActive(true);
                
                MarketScript script = marketContent.GetComponent<MarketScript>();
                if(script.isInited()){
                    script.ReloadTableData();
                }
            }
        } else if(Input.GetKeyDown(KeyCode.I)){
            if(inventoryCanvas.activeSelf){
                inventoryCanvas.SetActive(false);
            } else {
                inventoryCanvas.SetActive(true);
                
                InventoryScript script = inventoryContent.GetComponent<InventoryScript>();
                if(script.isInited()){
                    script.ReloadTableData();
                }
            }
        }
    }

    private void FixedUpdate() {
        if(movementInput != Vector2.zero) {
            bool success = TryMove(movementInput);
            updateMoveAnimation(success);
            if(!success){
                success = TryMove(new Vector2(movementInput.x, 0));
                updateMoveAnimation(success);
                if(!success) {
                    success = TryMove(new Vector2(0, movementInput.y));
                    updateMoveAnimation(success);
                }
            }
        } else {
            stopMoveAnimation();
        }
    }

    private void stopMoveAnimation(){
        animator.SetBool("isMoving", false);
    }

    private void updateMoveAnimation(bool value){
        if(value){
            animator.SetBool("isMoving", value);
            animator.SetFloat("X", movementInput.x);
            animator.SetFloat("Y", movementInput.y);
        }
    }

    private bool TryMove(Vector2 direction){
            int count = rb.Cast(
                direction,
                movementFilter,
                castCollisions,
                moveSpeed * Time.fixedDeltaTime + collisionOffset
            );
            if(count == 0){
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
                return true;
            } else {
                return false;
            }
    }


    void OnMove(InputValue movementValue) {
        movementInput = movementValue.Get<Vector2>();
    }

    void OnTriggerEnter2D(Collider2D col) {
        if(col.CompareTag("Item")){
            //Debug.Log(col.gameObject.name + " : " + gameObject.name + " : " + Time.time);
            ItemScript item = col.gameObject.GetComponent<ItemScript>();
            TokenEntity token = RestApi.CreateToken(item.itemName, item.itemUri);
            items.Add(token);
            Destroy(col.gameObject);
        }
    }
}
