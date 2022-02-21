using System;
using static System.Console;
using System.Text;

namespace Bme121
{
    record Player( string Name, string Colour, string Symbol);
    
    static class Program
    {
        static void Main( )
        {		
			Console.Clear();
			
			Welcome();
			
			Player[] players = new Player[2];
			
			WriteLine(" To use the default settings for the following settings, type <Enter>");
			WriteLine();
			
			string[] player1Info = new string[3];
			player1Info = CollectPlayerInfo(1, players);
			players[0] = new Player(player1Info[0], player1Info[1], player1Info[2]);
					
			string[] player2Info = new string[3];
			player2Info = CollectPlayerInfo(2, players);
			players[1] = new Player(player2Info[0], player2Info[1], player2Info[2]);
					
			int turn = GetFirstTurn(players);
			WriteLine();
					
			int rows = GetBoardSize("rows");
			int cols = GetBoardSize("columns");
			
			string[,] board = new string[rows,cols];
			board = NewBoard(rows, cols, players);
			
			bool gameOver = false;
			
			while(!gameOver)
			{
				Console.Clear();
				
				DisplayBoard(board);
				DisplayScore(board, players);
				
				string move = GetMove(players[turn]);
				bool madeMove = TryMove(board, players, move, turn);
				
				while(!madeMove)
				{
					Write(" Enter a different move: ");
					move = ReadLine();
					madeMove = TryMove(board, players, move, turn);
				}
					
				if(madeMove) board = ExecuteMove(board, players, move, turn);
				turn = (turn + 1) % players.Length;
				
				if(EndGame(move, board, players)) gameOver = true;
			}
			
			Console.Clear();
			
			DisplayBoard(board);
			DisplayScore(board, players);
			DisplayWinner(board, players, rows, cols);
			WriteLine();
        }
        
        
        static void Welcome()
        {
			WriteLine();
			WriteLine(" Welcome to Othello!");
			WriteLine();
		}
		
		
		static string[] CollectPlayerInfo(int playerNumber, Player[] players)
		{
			string[] playerInfo = new string[3];
				
			Write($" Enter the name of player {playerNumber}: ");
			playerInfo[0] = ReadLine();
			
			while(playerNumber == 2 && playerInfo[0] == players[0].Name || playerNumber == 2 && players[0].Name == "White" && playerInfo[0] == "")
			{
				Write(" You cannot have the same name as player 1, enter a different colour: ");
				playerInfo[0] = ReadLine();
			}
			
			if(playerInfo[0] == "" && playerNumber == 1) playerInfo[0] = "Black";
			if(playerInfo[0] == "" && playerNumber == 2) playerInfo[0] = "White";
				
			Write($" Enter the colour of player {playerNumber}: ");
			playerInfo[1] = ReadLine();
			
			while(playerNumber == 2 && playerInfo[1] == players[0].Colour || playerNumber == 2 && players[0].Colour == "white" && playerInfo[1] == "")
			{
				Write(" You cannot have the same colour as player 1, enter a different colour: ");
				playerInfo[1] = ReadLine();
			}
			
			if(playerInfo[1] == "" && playerNumber == 1) playerInfo[1] = "black";
			if(playerInfo[1] == "" && playerNumber == 2) playerInfo[1] = "white";			
				
			Write($" Enter the symbol of player {playerNumber}: ");
			playerInfo[2] = ReadLine();
			
			while(playerNumber == 2 && playerInfo[2] == players[0].Symbol || playerNumber == 2 && players[0].Symbol == "O" && playerInfo[2] == "")
			{
				Write(" You cannot have the same symbol as player 1, enter a different symbol: ");
				playerInfo[2] = ReadLine();
			}
			
			if(playerInfo[2] == "" && playerNumber == 1) playerInfo[2] = "X";
			if(playerInfo[2] == "" && playerNumber == 2) playerInfo[2] = "O";
			
			WriteLine();
			
			return playerInfo;
		}
		 
        
        static int GetFirstTurn(Player[] players)
        {
            Write(" Enter the name of the player who's turn is first: ");
            
            while(true)
            {
				string name = ReadLine();
				if(name == players[0].Name || name == "") return 0;
				if(name == players[1].Name) return 1;
				Write(" Unknown name entered, enter the name of player 1 or 2: ");
            }
        }
        
        
        static int GetBoardSize(string direction)
        {
            Write($" Enter the number of {direction}: ");
            string response = ReadLine();
            //made to be a string so if <Enter> is typed, it does not return an error
            
            if(response == "") return 8;
            
            int squares = int.Parse(response);
            
            while(squares < 4 || squares > 26 || squares % 2 == 1)
            {
				Write($" The number of squares must be between four and twenty-six and even-valued, enter the number of {direction}: ");
				squares = int.Parse(ReadLine());
			}
                 
            return squares;
        }
        
        
        static string[,] NewBoard(int rows, int cols, Player[] players)
        {
			string[,] newBoard = new string[rows, cols];
			
			for(int row = 0; row < rows; row ++)
                for(int col = 0; col < cols; col ++)
                    newBoard[row, col] = " ";
			
			newBoard[rows / 2 - 1, cols / 2 - 1] = players[0].Symbol;
			newBoard[rows / 2, cols / 2] = players[0].Symbol;
			newBoard[rows / 2 - 1, cols / 2] = players[1].Symbol;
			newBoard[rows / 2, cols / 2 - 1] = players[1].Symbol;
			
			return newBoard;
		}
		
		
		static int[] Score(string[,] board, Player[] players)
		{
			int[] score = new int[2];
			
			for(int row = 0; row < board.GetLength(0); row ++)
                for(int col = 0; col < board.GetLength(1); col ++)
				{
					if(board[row, col] == players[0].Symbol) score[0]++;
					if(board[row, col] == players[1].Symbol) score[1]++;
				}
				
			return score;
		}
		
		
		static void DisplayScore(string[,] board, Player[] players)
		{
			int[] score = Score(board, players);
			
			WriteLine();
			WriteLine($" Score: {players[0].Name} {score[0]}, {players[1].Name} {score[1]}");
			
			return; 
		}
		
		
		static string GetMove(Player players)
		{
			WriteLine();
			
			WriteLine($" It is {players.Name}'s turn, choose where to play a {players.Colour} disc ({players.Symbol})");
			WriteLine(" Pick a cell by its row then column name (e.g. 'bc') to play there.");
			WriteLine(" Use 'skip' to give up your turn. Use 'quit' to end the game.");
			Write(" Enter your move: ");
			
			return ReadLine();
		}
		
		
		static int[] Conversion(string[,] board, string move)
        {
			char[] alphabet = new char[26]
			{'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z'};
			
			char[] splitMove = move.ToCharArray();
			
			int[] convertedMove = new int[2] {-1, -1};
			
			for(int rows = 0; rows < board.GetLength(0); rows++)
				if(alphabet[rows] == splitMove[0]) convertedMove[0] = rows;
				
			for(int cols = 0; cols < board.GetLength(1); cols++)	
				if(alphabet[cols] == splitMove[1]) convertedMove[1] = cols;
				
			return convertedMove;
		}
        
        
        static bool TryMove(string[,] board, Player[] players, string move, int turn)
        {
			if(move == "skip" || move == "quit") return true;
			if(move.Length != 2) 
			{
				WriteLine(" The move should be two characters, one for the row and one for the column.");
				return false;
			}

			int[] convertedMove = Conversion(board, move);
			int row = convertedMove[0];
			int col = convertedMove[1];
			
			if(row == -1 || col == -1)
			{
				WriteLine(" The characters entered must be lowercase and present above one of the columns.");
				return false;
			}
			
			if(board[row,col] != " ")
			{
				WriteLine(" The spot selected to play at is already taken.");
				return false;
			}
			
			int i = 1;
			bool tryMove = false;
			
			while(!tryMove && i < 9)
			{
				int[] targetSquare = TryDirection(i, row, col, board, players, turn);
				
				if(targetSquare[0] != row || targetSquare[1] != col) tryMove = true;
				
				i++;
			}
			
			return tryMove;
		}
		
		
		static int[] TryDirection(int i, int row, int col, string[,] board, Player[] players, int turn)
		{
			int upSquares = row;
			int downSquares = board.GetLength(0) - row - 1;
			int rightSquares = board.GetLength(1) - col - 1;
			int leftSquares = col;
			int[] tryDirection = new int[2];
			tryDirection[0] = row;
			tryDirection[1] = col;
			
			if(i == 1)
			{
				int y = 1;
				
				if(upSquares == 1 || upSquares == 0) return tryDirection;
				if(board[row - 1, col] == players[turn].Symbol || board[row - 1, col] == " ") return tryDirection;
				
				while(board[row - y, col] == players[(turn + 1) % 2].Symbol && y < upSquares)
					y++;
				
				if(board[row - y, col] == players[turn].Symbol && y > 1)
				{
					tryDirection[0] = row - y;
					tryDirection[1] = col;
					
					return tryDirection;
				}	
				else return tryDirection;
			}
			if(i == 2)
			{
				int y = 1;
				int x = 1;
				
				if(upSquares == 1 || upSquares == 0 || rightSquares == 1 || rightSquares == 0) return tryDirection;
				if(board[row - 1, col + 1] == players[turn].Symbol || board[row - 1, col + 1] == " ") return tryDirection;
				
				while(board[row - y, col + x] == players[(turn + 1) % 2].Symbol && y < upSquares && x < rightSquares)
				{
					y++;
					x++;
				}
				
				if(board[row - y, col + x] == players[turn].Symbol && x > 1 && y > 1)
				{
					tryDirection[0] = row - y;
					tryDirection[1] = col + x;
					
					return tryDirection;
				}
				else return tryDirection;
			}
			if(i == 3)
			{
				int x = 1;
				
				if(rightSquares == 1 || rightSquares == 0) return tryDirection;
				if(board[row, col + 1] == players[turn].Symbol || board[row, col + 1] == " ") return tryDirection;
				
				while(board[row, col + x] == players[(turn + 1) % 2].Symbol && x < rightSquares)
					x++;
				
				if(board[row, col + x] == players[turn].Symbol && x > 1)
				{
					tryDirection[0] = row;
					tryDirection[1] = col + x;
					
					return tryDirection;
				}
				else return tryDirection;
			}
			if(i == 4)
			{
				int y = 1;
				int x = 1;
				
				if(rightSquares == 1 || rightSquares == 0 || downSquares == 1 || downSquares == 0) return tryDirection;
				if(board[row + 1, col + 1] == players[turn].Symbol || board[row + 1, col + 1] == " ") return tryDirection;
				
				while(board[row + y, col + x] == players[(turn + 1) % 2].Symbol && x < rightSquares && y < downSquares)
				{
					y++;
					x++;
				}
				
				if(board[row + y, col + x] == players[turn].Symbol && x > 1 && y > 1) 
				{
					tryDirection[0] = row + y;
					tryDirection[1] = col + x;
					
					return tryDirection;
				}
				else return tryDirection;
			}
			if(i == 5)
			{
				int y = 1;
				
				if(downSquares == 1 || downSquares == 0) return tryDirection;
				if(board[row + 1, col] == players[turn].Symbol || board[row + 1, col] == " ") return tryDirection;
				
				while(board[row + y, col] == players[(turn + 1) % 2].Symbol && y < downSquares)
					y++;
				
				if(board[row + y, col] == players[turn].Symbol && y > 1)
				{
					tryDirection[0] = row + y;
					tryDirection[1] = col;
					
					return tryDirection;
				}
				else return tryDirection;
			}
			if(i == 6)
			{
				int y = 1;
				int x = 1;
				
				if(downSquares == 1 || downSquares == 0 || leftSquares == 1 || leftSquares == 0) return tryDirection;
				if(board[row + 1, col - 1] == players[turn].Symbol || board[row + 1, col - 1] == " ") return tryDirection;
				
				while(board[row + y, col - x] == players[(turn + 1) % 2].Symbol && y < downSquares && x < leftSquares)
				{
					y++;
					x++;
				}
				
				if(board[row + y, col - x] == players[turn].Symbol && x > 1 && y > 1)
				{
					tryDirection[0] = row + y;
					tryDirection[1] = col - x;
					
					return tryDirection;
				}
				else return tryDirection;
			}
			if(i == 7)
			{
				int x = 1;
				
				if(leftSquares == 1 || leftSquares == 0) return tryDirection;
				if(board[row, col - 1] == players[turn].Symbol || board[row, col - 1] == " ") return tryDirection;
				
				while(board[row, col - x] == players[(turn + 1) % 2].Symbol && x < leftSquares)
					x++;
				
				if(board[row, col - x] == players[turn].Symbol && x > 1)
				{
					tryDirection[0] = row;
					tryDirection[1] = col - x;
					
					return tryDirection;
				}
				else return tryDirection;
			}
			if(i == 8)
			{
				int y = 1;
				int x = 1;
				
				if(leftSquares == 1 || leftSquares == 0 || upSquares == 1 || upSquares == 0) return tryDirection;
				if(board[row - 1, col - 1] == players[turn].Symbol || board[row - 1, col - 1] == " ") return tryDirection;
				
				while(board[row - y, col - x] == players[(turn + 1) % 2].Symbol && y < upSquares && x < leftSquares)
				{
					y++;
					x++;
				}
				
				if(board[row - y, col - x] == players[turn].Symbol && x > 1 && y > 1)
				{
					tryDirection[0] = row - y;
					tryDirection[1] = col - x;
					
					return tryDirection;
				}
				else return tryDirection;
			}
			return tryDirection;
		}
		
		
		static string[,] ExecuteMove(string[,] board, Player[] players, string move, int turn)
		{
			if(move == "quit" || move == "skip") return board;
			
			int[] convertedMove = Conversion(board, move);
			int row = convertedMove[0];
			int col = convertedMove[1];
			
			for(int i = 1; i < 9; i++)
			{
				int[] targetSquare = TryDirection(i, row, col, board, players, turn);
				
				if(targetSquare[0] != row && i == 1 || targetSquare[1] != col && i == 1)
				{
					for(int r1 = 0; row - r1 > targetSquare[0]; r1++)
					{
						board[row - r1, col] = players[turn].Symbol;
					}
				}
				if(targetSquare[0] != row && i == 2 || targetSquare[1] != col && i == 2)
				{
					int r2 = 0;
					int c2 = 0;
					
					while(row - r2 > targetSquare[0] && col + c2 < targetSquare[1])
					{
						board[row - r2, col + c2] = players[turn].Symbol;
						
						r2++;
						c2++;
					}
				}
				if(targetSquare[0] != row && i == 3 || targetSquare[1] != col && i == 3)
				{
					for(int c3 = 0; col + c3 < targetSquare[1]; c3++)
					{
						board[row, col + c3] = players[turn].Symbol;
					}
				}
				if(targetSquare[0] != row && i == 4 || targetSquare[1] != col && i == 4)
				{
					int r4 = 0;
					int c4 = 0;
					
					while(row + r4 < targetSquare[0] && col + c4 < targetSquare[1])
					{
						board[row + r4, col + c4] = players[turn].Symbol;
						
						r4++;
						c4++;
					}
				}
				if(targetSquare[0] != row && i == 5 || targetSquare[1] != col && i == 5)
				{
					for(int r5 = 0; row + r5 < targetSquare[0]; r5++)
					{
						board[row + r5, col] = players[turn].Symbol;
					}
				}
				if(targetSquare[0] != row && i == 6 || targetSquare[1] != col && i == 6)
				{
					int r6 = 0;
					int c6 = 0;
					
					while(row + r6 < targetSquare[0] && col - c6 > targetSquare[1])
					{
						board[row + r6, col - c6] = players[turn].Symbol;
						
						r6++;
						c6++;
					}
				}
				if(targetSquare[0] != row && i == 7 || targetSquare[1] != col && i == 7)
				{
					for(int c7 = 0; col - c7 > targetSquare[1]; c7++)
					{
						board[row, col - c7] = players[turn].Symbol;
					}
				}
				if(targetSquare[0] != row && i == 8 || targetSquare[1] != col && i == 8)
				{
					int r8 = 0;
					int c8 = 0;
					
					while(row - r8 > targetSquare[0] && col - c8 > targetSquare[1])
					{
						board[row - r8, col - c8] = players[turn].Symbol;
						
						r8++;
						c8++;
					}
				}
			}
			
			return board;
		}
		
        
		static bool EndGame(string move, string[,] board, Player[] players)
        {
			bool isEnd = false;
			
			if(move == "quit" || FullBoard(board) || NoMove(board, players))
				isEnd = true;
			
			return isEnd;
		}
		
		
		static bool FullBoard(string[,] board)
		{
			bool isFull = true;
			
			for(int row = 0; row < board.GetLength(0); row ++)
                for(int col = 0; col < board.GetLength(1); col ++)
                    if(board[row, col] == " ") isFull = false;
			
			return isFull;
		}
		
		
		static bool NoMove(string[,] board, Player[] players)
		{
			bool move1 = false;
			bool move2 = false;
			
			for(int row = 0; row < board.GetLength(0); row ++)
                for(int col = 0; col < board.GetLength(1); col ++)
					if(board[row,col] == " ")
					{
						int i = 0;
						
						while(!move1 && i < 9)
						{
							int[] targetSquare = TryDirection(i, row, col, board, players, 0);
							
							if(targetSquare[0] != row || targetSquare[1] != col) move1 = true;
							
							i++;
						}
						
						if(move1) return false;
					}
					
			for(int row = 0; row < board.GetLength(0); row ++)
                for(int col = 0; col < board.GetLength(1); col ++)
					if(board[row,col] == " ")
					{
						int i = 0;
						
						while(!move2 && i < 9)
						{
							int[] targetSquare = TryDirection(i, row, col, board, players, 1);
							
							if(targetSquare[0] != row || targetSquare[1] != col) move2 = true;
							
							i++;
						}
						
						if(move2) return false;
					}
            
			if(move1 == false && move2 == false) return true;
			else return false;
		}       
        
        
		static void DisplayWinner(string[,] board, Player[] players, int rows, int cols)
		{
			int[] score = Score(board, players);
			double difference = Math.Abs(score[0] - score[1]);
			double percentDifference = difference / (rows * cols) * 100;
			double bound1 = 53 * 100 / 64;
			double bound2 = 39 * 100 / 64;
			double bound3 = 25 * 100 / 64;
			double bound4 = 11 * 100 / 64;
			
			if(score[0] > score[1]) Write($" {players[0].Name} is the winner");
			if(score[1] > score[0]) Write($" {players[1].Name} is the winner");
			
			if(percentDifference <= 1 && percentDifference > bound1) WriteLine(" of a perfect game!");
			if(percentDifference <= bound1 && percentDifference > bound2) WriteLine(" of a walkaway game!");
			if(percentDifference <= bound2 && percentDifference > bound3) WriteLine(" of a fight game!");
			if(percentDifference <= bound3 && percentDifference > bound4) WriteLine(" of a hot game!");
			if(percentDifference <= bound4 && percentDifference > 0) WriteLine(" of a close game!");
			if(percentDifference == 0) WriteLine($" {players[0].Name} and {players[1].Name} have tied!");
			
			return;
		}
        
        
        static string LetterAtIndex( int number )
        {
            if( number < 0 || number > 25 ) return " ";
            else return "abcdefghijklmnopqrstuvwxyz"[ number ].ToString( );
        }
        
        // -----------------------------------------------------------------------------------------
        // Return the index 0..25 corresponding to its single-character string "a".."z". 
        // Return -1 for an invalid string.
        
        static int IndexAtLetter( string letter )
        {
            if( letter.Length != 1 ) return -1;
            else return "abcdefghijklmnopqrstuvwxyz".IndexOf( letter[ 0 ] );
        }

        // -----------------------------------------------------------------------------------------
        // Display the Othello game board on the Console.
        // All information about the game is held in the two-dimensional string array.
        
        static void DisplayBoard( string[ , ] board )
        {
            const string h  = "\u2500"; // horizontal line
            const string v  = "\u2502"; // vertical line
            const string tl = "\u250c"; // top left corner
            const string tr = "\u2510"; // top right corner
            const string bl = "\u2514"; // bottom left corner
            const string br = "\u2518"; // bottom right corner
            const string vr = "\u251c"; // vertical join from right
            const string vl = "\u2524"; // vertical join from left
            const string hb = "\u252c"; // horizontal join from below
            const string ha = "\u2534"; // horizontal join from above
            const string hv = "\u253c"; // horizontal vertical cross
            const string mx = "\u256c"; // marked horizontal vertical cross
            const string sp =      " "; // space

            // Nothing to display?
            if( board == null ) return;
            
            int rows = board.GetLength( 0 );
            int cols = board.GetLength( 1 );
            if( rows == 0 || cols == 0 ) return;
            
            // Display the board row by row.
            for( int row = 0; row < rows; row ++ )
            {
                if( row == 0 )
                {
                    // Labels above top edge.
                    for( int col = 0; col < cols; col ++ )
                    {
                        if( col == 0 ) Write( "   {0}{0}{1}{0}", sp, LetterAtIndex( col ) );
                        else Write( "{0}{0}{1}{0}", sp, LetterAtIndex( col ) );
                    }
                    WriteLine( );
                    
                    // Border above top row.
                    for( int col = 0; col < cols; col ++ )
                    {
                        if( col == 0 ) Write( "   {0}{1}{1}{1}", tl, h );
                        else Write( "{0}{1}{1}{1}", hb, h );
                        if( col == cols - 1 ) Write( "{0}", tr );
                    }
                    WriteLine( );
                }
                else
                {
                    // Border above a row which is not the top row.
                    for( int col = 0; col < cols; col ++ )
                    {
                        if(    rows > 5 && cols > 5 && row ==        2 && col ==        2 
                            || rows > 5 && cols > 5 && row ==        2 && col == cols - 2 
                            || rows > 5 && cols > 5 && row == rows - 2 && col ==        2 
                            || rows > 5 && cols > 5 && row == rows - 2 && col == cols - 2 )  
                            Write( "{0}{1}{1}{1}", mx, h );
                        else if( col == 0 ) Write( "   {0}{1}{1}{1}", vr, h );
                        else Write( "{0}{1}{1}{1}", hv, h );
                        if( col == cols - 1 ) Write( "{0}", vl );
                    }
                    WriteLine( );
                }
                
                // Row content displayed column by column.
                for( int col = 0; col < cols; col ++ ) 
                {
                    if( col == 0 ) Write( " {0,-2}", LetterAtIndex( row ) ); // Labels on left side
                    Write( "{0} {1} ", v, board[ row, col ] );
                    if( col == cols - 1 ) Write( "{0}", v );
                }
                WriteLine( );
                
                if( row == rows - 1 )
                {
                    // Border below last row.
                    for( int col = 0; col < cols; col ++ )
                    {
                        if( col == 0 ) Write( "   {0}{1}{1}{1}", bl, h );
                        else Write( "{0}{1}{1}{1}", ha, h );
                        if( col == cols - 1 ) Write( "{0}", br );
                    }
                    WriteLine( );
                }
            }
        }
        
    }
}
