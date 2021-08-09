using UnityEngine;
using System.Collections.Generic;

public enum SpecialMove{
    None = 0,
    EnPassant,
    Castling,
    Promotion
}


public class Chessboard : MonoBehaviour{

    [Header("Assets")]
    [SerializeField]private Sprite darkTile;
    [SerializeField]private Sprite lightTile;
    [SerializeField]private Sprite hoverTile;
    [SerializeField]private Sprite highlightTile;

    [Header("Chess Pieces")]
    [SerializeField]private Vector3 pieceScale = new Vector3(1.2f, 1.2f, 1f);
    [SerializeField]private GameObject[] whitePrefabs;
    [SerializeField]private GameObject[] blackPrefabs;

    private GameObject[,] tiles;
    private ChessPiece[,] piecesOnBoard;
    private ChessPiece draggedPiece;

    private List<ChessPiece> whiteDeaths = new List<ChessPiece>();
    private List<ChessPiece> blackDeaths = new List<ChessPiece>();

    private List<Vector2Int> availableMoves = new List<Vector2Int>();

    //Special move concept
    private List<Vector2Int[]> moveList = new List<Vector2Int[]>(); // The list of moves, Vector2Int[] stores previous position, and new position
    private SpecialMove specialMove;

    private TeamPlayer currentPlayer;


    private const int BOARD_SIZE = 8;
    private float tileSize;

    private Camera camera;
    private Vector2Int currentHover;


    [HideInInspector] public bool isBlackWinner = false;
    [HideInInspector] public bool isWhiteWinner = false;

    private void Awake() {
        currentPlayer = TeamPlayer.White;    
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
                                                    LayerMask.GetMask("Tile", "Hover", "Highlighted"), 0, 5);

        if(collider != null){
            
            GameObject tile = collider.gameObject;
            Vector2Int hoveredTile = WhichTile(tile);

            if(currentHover == -Vector2Int.one){
                currentHover = hoveredTile;
                tiles[hoveredTile.x, hoveredTile.y].layer = LayerMask.NameToLayer("Hover");
            }
            
            if(currentHover != hoveredTile){
                tiles[currentHover.x, currentHover.y].layer = (ShouldBeHighlighted(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlighted") : LayerMask.NameToLayer("Tile");
                UpdateTile(currentHover);
                currentHover = hoveredTile;
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Hover");
             }

             tiles[hoveredTile.x, hoveredTile.y].GetComponent<SpriteRenderer>().sprite = hoverTile;

    
            if(Input.GetMouseButtonDown(0)){
                if(piecesOnBoard[hoveredTile.x, hoveredTile.y] != null){
                    draggedPiece = piecesOnBoard[hoveredTile.x, hoveredTile.y];

                    // Check if right player chose his piece
                    if(draggedPiece.team != currentPlayer){  
                        draggedPiece = null;
                        return;
                    }

                    draggedPiece.gameObject.transform.localScale = pieceScale;

                    // Getting list of available moves, and highlighting right tiles
                    availableMoves = draggedPiece.GetAvailableMoves(ref piecesOnBoard, BOARD_SIZE);
                    specialMove = draggedPiece.GetSpecialMove(ref piecesOnBoard, ref moveList, ref availableMoves);
                    HighlightTiles();
                }
            }

            if(draggedPiece != null && Input.GetMouseButtonUp(0)){
                Vector2Int previous = new Vector2Int(draggedPiece.currentX, draggedPiece.currentY);
                bool isMoveValid = isMoveAvailable(draggedPiece, currentHover.x, currentHover.y);

                if(isMoveValid){
                    MakeSpecialMove();
                    currentPlayer = (currentPlayer == TeamPlayer.White) ? TeamPlayer.Black : TeamPlayer.White;
                }

                draggedPiece.gameObject.transform.localScale = new Vector3(1f,1f,1f);
                draggedPiece = null;
                RemoveHighlightTiles();
            }

        }else{
            if(currentHover != -Vector2Int.one){
                GameObject tile = tiles[currentHover.x, currentHover.y];
                tile.layer = (ShouldBeHighlighted(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlighted") : LayerMask.NameToLayer("Tile"); 
                UpdateTile(currentHover);
                currentHover = -Vector2Int.one;
            }
        }
        #endregion
    }


    private void MakeSpecialMove(){

        if(specialMove == SpecialMove.EnPassant){
            dynamic newPosition = moveList[moveList.Count - 1];
            dynamic targetPosition = moveList[moveList.Count - 2];

            ChessPiece capturingPawn = piecesOnBoard[newPosition[1].x, newPosition[1].y];
            ChessPiece capturedPawn = piecesOnBoard[targetPosition[1].x, targetPosition[1].y];

            if(capturingPawn.currentX == capturedPawn.currentX){
                if(capturedPawn.currentY == capturingPawn.currentY+1 || capturedPawn.currentY == capturingPawn.currentY-1){
                    Capture(capturingPawn, capturedPawn);
                }
            }
        }
        if(specialMove == SpecialMove.Castling){
            dynamic newPosition = moveList[moveList.Count - 1];

            int row = ((currentPlayer == TeamPlayer.White) ? 0 : 7);

            // Left castling
            if(newPosition[0].x > newPosition[1].x){
                MoveTo(piecesOnBoard[0,row], 3, row); // move left rook
            }
            // Right castling
            else{
                MoveTo(piecesOnBoard[7,row], 5, row); // move right rook
            } 
        }
        if(specialMove == SpecialMove.Promotion){
            dynamic newPosition = moveList[moveList.Count - 1];
            bool isWhite = ((currentPlayer == TeamPlayer.White) ? true : false);

            Destroy(piecesOnBoard[newPosition[1].x, newPosition[1].y].gameObject);
            piecesOnBoard[newPosition[1].x, newPosition[1].y] = SpawnSinglePiece(isWhite, ChessPieceType.Queen);
            positionSinglePiece(newPosition[1].x, newPosition[1].y);

        }

    }

    /// <Summary>
    /// Moving pieceToMove on the x and y position
    /// </Summary>
    private void MoveTo(ChessPiece pieceToMove, int x, int y){
            Vector2Int previousHover = new Vector2Int(pieceToMove.currentX, pieceToMove.currentY);
            piecesOnBoard[previousHover.x, previousHover.y] = null;
            piecesOnBoard[x,y] = pieceToMove;
            positionSinglePiece(x, y);

            moveList.Add(new Vector2Int[] { new Vector2Int(previousHover.x, previousHover.y), new Vector2Int(x, y)});

            pieceToMove.gameObject.transform.localScale = new Vector3(1f,1f,1f);
    }
    /// <Summary>
    /// Capturing pieces and move captured on the side of board
    /// </Summary>
    private void Capture(ChessPiece capturing, ChessPiece captured){
        piecesOnBoard[captured.currentX, captured.currentY] = null;

        if(captured.team == TeamPlayer.White){
            if(captured.type == ChessPieceType.King){
                isBlackWinner = true;
                return;
            }
            whiteDeaths.Add(captured);
            captured.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            captured.gameObject.transform.position = new Vector2(36f, ((whiteDeaths.Count - 2) * tileSize/2) + 1f);
        }else{
            if(captured.type == ChessPieceType.King){
                isWhiteWinner = true;
                return;
            }
            blackDeaths.Add(captured);
            captured.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            captured.gameObject.transform.position = new Vector2(-5f, 31.57f - ((blackDeaths.Count - 2) * tileSize/2) - 1f);
        }
    }

    /// <Summary>
    /// Checks if pieceToMove can move on position (x,y),
    /// and also doing this with function MoveTo, and Capture
    /// Return false if move is not possible
    /// </Summary>
    private bool isMoveAvailable(ChessPiece pieceToMove, int x, int y){
        ChessPiece otherPiece = piecesOnBoard[x, y];

        // Check if player has chosen available tile, if not, move is not possible
        if(!ShouldBeHighlighted(ref availableMoves, new Vector2Int(x,y)))
            return false;

        if(otherPiece == null){
            MoveTo(pieceToMove, x, y);
            return true;
        }

        if(otherPiece != null){
            if(otherPiece.team != pieceToMove.team){
                Capture(pieceToMove, otherPiece);
                MoveTo(pieceToMove, x, y);
                return true;
            }
            else{
                pieceToMove.gameObject.transform.localScale = new Vector3(1f,1f,1f);
                return false;
            }
        }

        return false;
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

        if(renderer.gameObject.layer == LayerMask.NameToLayer("Highlighted"))
        renderer.sprite = highlightTile;

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

    /// <Summary>
    /// This method spawns all chess pieces
    /// </Summary>
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
        piecesOnBoard[6,7] = SpawnSinglePiece(false, ChessPieceType.Knight);
        piecesOnBoard[7,7] = SpawnSinglePiece(false, ChessPieceType.Rook);
        for(int i =0; i < BOARD_SIZE; ++i){
            piecesOnBoard[i, 6] = SpawnSinglePiece(false, ChessPieceType.Pawn);
        }

    }

    /// <Summary>
    /// Spawns single chess piece 
    /// bool isWhite - checks if piece is white or not
    /// <Summary>
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


    /// <Summary>
    /// Positioning all of pieces in piecesOnBoard array on the right positions
    /// <Summary>
    private void positionAllPieces(){
        for(int x = 0; x < BOARD_SIZE; x++)
            for(int y = 0; y < BOARD_SIZE; y++)
                if(piecesOnBoard[x,y] != null)
                    positionSinglePiece(x,y);

    }
    /// <Summary>
    /// Positioning single piece which is on (x,y) position in array piecesOnBoard
    /// on the x and y (multiplied by tileSize) position on the board
    /// </Summary>
    private void positionSinglePiece(int x, int y){
        piecesOnBoard[x,y].currentX = x;
        piecesOnBoard[x,y].currentY = y;
        piecesOnBoard[x,y].transform.position = new Vector2(x*tileSize,y*tileSize);
    }
    

    private void HighlightTiles(){
        for(int i = 0; i < availableMoves.Count; ++i){
            GameObject tile = tiles[availableMoves[i].x, availableMoves[i].y];
            tile.layer = LayerMask.NameToLayer("Highlighted");
            tile.GetComponent<SpriteRenderer>().sprite = highlightTile;
        }
    }

    private void RemoveHighlightTiles(){
        for(int i = 0; i < availableMoves.Count; ++i){
            GameObject tile = tiles[availableMoves[i].x, availableMoves[i].y];
            tile.layer = LayerMask.NameToLayer("Tile");
            UpdateTile(new Vector2Int(availableMoves[i].x, availableMoves[i].y));
        }

        availableMoves.Clear();
    }

    private bool ShouldBeHighlighted(ref List<Vector2Int> moves, Vector2Int hover){
        for(int i = 0; i < moves.Count; ++i){
            if(moves[i].x == hover.x && moves[i].y == hover.y)
                return true;
        }

        return false;
    }

}
