using UnityEngine;

public class Chessboard : MonoBehaviour{
    [Header("Assets")]
    [SerializeField]private Sprite darkTile;
    [SerializeField]private Sprite lightTile;
    [SerializeField]private Sprite hoverTile;
    [SerializeField]private Sprite clickedTile;

    [Header("Chess Pieces")]
    [SerializeField]private Vector3 pieceScale = new Vector3(1.2f, 1.2f, 1f);
    [SerializeField]private GameObject[] whitePrefabs;
    [SerializeField]private GameObject[] blackPrefabs;

    private GameObject[,] tiles;
    private ChessPiece[,] piecesOnBoard;
    private ChessPiece draggedPiece;


    private const int BOARD_SIZE = 8;
    private float tileSize;

    private Camera camera;
    private Vector2Int currentHover;

    private void Awake() {        
        currentHover = -Vector2Int.one;
        GenerateBoard();

        SpawnAllPieces();
        positionAllPieces();
    }

    private void Update(){

        if(!camera){
            camera = Camera.main;
            return;
        }


        /// code below detects which tile is hovered
        #region hoverCheck 
        Collider2D collider = Physics2D.OverlapPoint(camera.ScreenToWorldPoint(Input.mousePosition),  
                                                    LayerMask.GetMask("Tile", "Hover"), 0, 5);

        if(collider != null){
            
            GameObject tile = collider.gameObject;
            Vector2Int hoveredTile = WhichTile(tile);

            if(currentHover == -Vector2Int.one){
                currentHover = hoveredTile;
                tiles[hoveredTile.x, hoveredTile.y].layer = LayerMask.NameToLayer("Hover");
            }
            
            if(currentHover != hoveredTile){
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                UpdateTile(currentHover);
                currentHover = hoveredTile;
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Hover");
             }

             tiles[hoveredTile.x, hoveredTile.y].GetComponent<SpriteRenderer>().sprite = hoverTile;

    
            if(Input.GetMouseButtonDown(0)){
                if(piecesOnBoard[hoveredTile.x, hoveredTile.y] != null){
                    draggedPiece = piecesOnBoard[hoveredTile.x, hoveredTile.y];
                    draggedPiece.gameObject.transform.localScale = pieceScale;
                }
            }

            if(Input.GetMouseButtonUp(0)){
                if(draggedPiece != null && piecesOnBoard[currentHover.x, currentHover.y] == null){
                    Vector2Int previousHover = new Vector2Int(draggedPiece.currentX, draggedPiece.currentY);
                    piecesOnBoard[previousHover.x, previousHover.y] = null;
                    draggedPiece.gameObject.transform.position = tiles[currentHover.x, currentHover.y].transform.position;
                    draggedPiece.currentX = currentHover.x;
                    draggedPiece.currentY = currentHover.y;
                    piecesOnBoard[currentHover.x, currentHover.y] = draggedPiece;
                    draggedPiece.gameObject.transform.localScale = new Vector3(1f,1f,1f);
                    draggedPiece = null;
                }
            }


        }else{
            if(currentHover != -Vector2Int.one){
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                UpdateTile(currentHover);
                currentHover = -Vector2Int.one;
            }
        }
        #endregion
    }


    ///<Summary>
    /// Function checks if gameobject given in parameter is in the tiles array (chessboard)
    /// if so, it returns coordinates of that tile
    /// else returns (-1,-1)
    ///</Summary>
    private Vector2Int WhichTile(GameObject tileHitted){
        for(int x = 0; x < BOARD_SIZE; x++)
            for(int y = 0; y < BOARD_SIZE; y++)
                if(tiles[x, y] == tileHitted)
                    return new Vector2Int(x,y);
        
        return -Vector2Int.one;
    }

    ///<Summary>
    /// This method changes sprite of the tile which is on coordinates given in parameter
    /// to sprite of the right tile
    ///</Summary>
    private void UpdateTile(Vector2Int currentHover){
        SpriteRenderer renderer = tiles[currentHover.x, currentHover.y].GetComponent<SpriteRenderer>();
        if((currentHover.x + currentHover.y) % 2 == 0) renderer.sprite = darkTile;
        else renderer.sprite = lightTile;

    }


    /// <Summary>
    /// Method <c>GenerateBoard()</c> generates all game board,
    /// and fill <c>tiles</c> array.
    /// </Summary>
    private void GenerateBoard(){
        tiles = new GameObject[BOARD_SIZE, BOARD_SIZE];

        for(int x = 0; x < BOARD_SIZE; x++)
            for(int y = 0; y < BOARD_SIZE; y++)
                tiles[x, y] = GenerateTile(x, y);
    }

    /// <Summary>
    /// Method <c>GenerateTile()</c> create one tile,
    /// gives it right sprite, generate a collider, and draw on screen.
    /// It's also return this tile.
    /// </Summary>
    private GameObject GenerateTile(float x, float y){
        // Generate tile
        GameObject tile = new GameObject(string.Format("[ {0}, {1} ]", x, y));
        tile.transform.parent = transform;

        // Which tile should be drawed, light or dark
        if((x+y)%2 == 0) tile.AddComponent<SpriteRenderer>().sprite = darkTile;
        else tile.AddComponent<SpriteRenderer>().sprite = lightTile;
        
        // Add collider and right layer (tile)
        tile.AddComponent<BoxCollider2D>();
        tile.layer = LayerMask.NameToLayer("Tile");

        // Draw in right place
        SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
        tileSize = spriteRenderer.size.x;
        tile.transform.position = new Vector2(x*spriteRenderer.size.x, y*spriteRenderer.size.x);
        

        return tile;
    }

    private void SpawnAllPieces(){
        piecesOnBoard = new ChessPiece[BOARD_SIZE, BOARD_SIZE];
        
        // White
        piecesOnBoard[0,0] = SpawnSinglePiece(true, ChessPieceType.Rook);
        piecesOnBoard[1,0] = SpawnSinglePiece(true, ChessPieceType.Knight);
        piecesOnBoard[2,0] = SpawnSinglePiece(true, ChessPieceType.Bishop);
        piecesOnBoard[3,0] = SpawnSinglePiece(true, ChessPieceType.Queen);
        piecesOnBoard[4,0] = SpawnSinglePiece(true, ChessPieceType.King);
        piecesOnBoard[5,0] = SpawnSinglePiece(true, ChessPieceType.Bishop);
        piecesOnBoard[6,0] = SpawnSinglePiece(true, ChessPieceType.Knight);
        piecesOnBoard[7,0] = SpawnSinglePiece(true, ChessPieceType.Rook);
        for(int i =0; i < BOARD_SIZE; ++i){
            piecesOnBoard[i, 1] = SpawnSinglePiece(true, ChessPieceType.Pawn);
        }

        // Black
        piecesOnBoard[0,7] = SpawnSinglePiece(false, ChessPieceType.Rook);
        piecesOnBoard[1,7] = SpawnSinglePiece(false, ChessPieceType.Knight);
        piecesOnBoard[2,7] = SpawnSinglePiece(false, ChessPieceType.Bishop);
        piecesOnBoard[3,7] = SpawnSinglePiece(false, ChessPieceType.Queen);
        piecesOnBoard[4,7] = SpawnSinglePiece(false, ChessPieceType.King);
        piecesOnBoard[5,7] = SpawnSinglePiece(false, ChessPieceType.Bishop);
        piecesOnBoard[6,7] = SpawnSinglePiece(false, ChessPieceType.Rook);
        piecesOnBoard[7,7] = SpawnSinglePiece(false, ChessPieceType.Knight);
        for(int i =0; i < BOARD_SIZE; ++i){
            piecesOnBoard[i, 6] = SpawnSinglePiece(false, ChessPieceType.Pawn);
        }

    }

    private ChessPiece SpawnSinglePiece(bool isWhite, ChessPieceType type){
        ChessPiece piece;
        if(isWhite){ 
            piece = Instantiate(whitePrefabs[(int)type - 1], transform).GetComponent<ChessPiece>();
            piece.team = TeamPlayer.White;
        }else{ 
            piece = Instantiate(blackPrefabs[(int)type - 1], transform).GetComponent<ChessPiece>();
            piece.team = TeamPlayer.Black;
        }

        piece.type = type;

        return piece;

    }


    private void positionAllPieces(){
        for(int x = 0; x < BOARD_SIZE; x++)
            for(int y = 0; y < BOARD_SIZE; y++)
                if(piecesOnBoard[x,y] != null)
                    positionSinglePiece(x,y);

    }
    private void positionSinglePiece(int x, int y, bool smoothMove = false){
        piecesOnBoard[x,y].currentX = x;
        piecesOnBoard[x,y].currentY = y;
        piecesOnBoard[x,y].transform.position = new Vector2(x*tileSize,y*tileSize);
    }


    



}
