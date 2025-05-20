
    let board = [];
    let size = 3;
    let gameActive = true;
    let minimaxLog = [];

    // Initialize the game
    function initGame() {
      size = parseInt(document.getElementById('boardSize').value);
      board = Array(size * size).fill(null);
      createBoard();
      gameActive = true;
      document.getElementById('status').textContent = "Your turn (X)";
      document.getElementById('log').value = "Minimax Log will appear here...";
    }

    // Create the board UI
    function createBoard() {
      const boardElement = document.getElementById('board');
      boardElement.style.gridTemplateColumns = `repeat(${size}, 80px)`;
      boardElement.innerHTML = '';
      for (let i = 0; i < size * size; i++) {
        const cell = document.createElement('div');
        cell.classList.add('cell');
        cell.dataset.index = i;
        cell.addEventListener('click', () => handleClick(i));
        boardElement.appendChild(cell);
      }
      updateBoard();
    }

    // Update board UI
    function updateBoard() {
      const cells = document.querySelectorAll('.cell');
      cells.forEach((cell, i) => {
        cell.textContent = board[i] || '';
      });
    }

    // Handle player move
    function handleClick(index) {
      if (!gameActive || board[index] !== null) return;
      board[index] = 'X';
      updateBoard();
      const result = checkWinner();
      if (result) {
        endGame(result);
        return;
      }
      aiMove();
    }

    // Check for winner (3 in a row)
    function checkWinner() {
      // Check rows
      for (let i = 0; i < size; i++) {
        for (let j = 0; j <= size - 3; j++) {
          const start = i * size + j;
          if (board[start] && board[start] === board[start + 1] && board[start] === board[start + 2]) {
            return board[start];
          }
        }
      }
      // Check columns
      for (let i = 0; i <= size - 3; i++) {
        for (let j = 0; j < size; j++) {
          const start = i * size + j;
          if (board[start] && board[start] === board[start + size] && board[start] === board[start + 2 * size]) {
            return board[start];
          }
        }
      }
      // Check main diagonals
      for (let i = 0; i <= size - 3; i++) {
        for (let j = 0; j <= size - 3; j++) {
          const start = i * size + j;
          if (board[start] && board[start] === board[start + size + 1] && board[start] === board[start + 2 * (size + 1)]) {
            return board[start];
          }
        }
      }
      // Check anti-diagonals
      for (let i = 0; i <= size - 3; i++) {
        for (let j = 2; j < size; j++) {
          const start = i * size + j;
          if (board[start] && board[start] === board[start + size - 1] && board[start] === board[start + 2 * (size - 1)]) {
            return board[start];
          }
        }
      }
      // Check for tie
      return board.includes(null) ? null : 'tie';
    }

    // Format board for logging
    function formatBoard(board) {
      let result = '';
      for (let i = 0; i < size; i++) {
        for (let j = 0; j < size; j++) {
          result += (board[i * size + j] || '.') + ' ';
        }
        result += '\n';
      }
      return result;
    }

    // Minimax with Alpha-Beta pruning
    function minimax(board, depth, isMaximizing, alpha, beta) {
      const result = checkWinner();
      if (result) {
        const score = result === 'O' ? 10 - depth : result === 'X' ? -10 + depth : 0;
        minimaxLog.push(`Depth ${depth}, Terminal state:\n${formatBoard(board)}Score: ${score}`);
        return score;
      }

      if (isMaximizing) {
        let bestScore = -Infinity;
        for (let i = 0; i < size * size; i++) {
          if (board[i] === null) {
            board[i] = 'O';
            minimaxLog.push(`Depth ${depth}, AI tries move at ${i}:\n${formatBoard(board)}`);
            const score = minimax(board, depth + 1, false, alpha, beta);
            board[i] = null;
            bestScore = Math.max(bestScore, score);
            alpha = Math.max(alpha, bestScore);
            if (beta <= alpha) {
              minimaxLog.push(`Depth ${depth}, Prune at move ${i}, Alpha: ${alpha}, Beta: ${beta}`);
              break;
            }
          }
        }
        return bestScore;
      } else {
        let bestScore = Infinity;
        for (let i = 0; i < size * size; i++) {
          if (board[i] === null) {
            board[i] = 'X';
            minimaxLog.push(`Depth ${depth}, Player tries move at ${i}:\n${formatBoard(board)}`);
            const score = minimax(board, depth + 1, true, alpha, beta);
            board[i] = null;
            bestScore = Math.min(bestScore, score);
            beta = Math.min(beta, bestScore);
            if (beta <= alpha) {
              minimaxLog.push(`Depth ${depth}, Prune at move ${i}, Alpha: ${alpha}, Beta: ${beta}`);
              break;
            }
          }
        }
        return bestScore;
      }
    }

    // AI move using Minimax with Alpha-Beta
    function aiMove() {
      minimaxLog = [];
      let bestScore = -Infinity;
      let move;
      for (let i = 0; i < size * size; i++) {
        if (board[i] === null) {
          board[i] = 'O';
          minimaxLog.push(`Evaluating move at ${i}:\n${formatBoard(board)}`);
          const score = minimax(board, 0, false, -Infinity, Infinity);
          board[i] = null;
          if (score > bestScore) {
            bestScore = score;
            move = i;
          }
          minimaxLog.push(`Move ${i} score: ${score}`);
        }
      }
      board[move] = 'O';
      minimaxLog.push(`AI chooses move at ${move} with score ${bestScore}`);
      document.getElementById('log').value = minimaxLog.join('\n\n');
      updateBoard();
      const result = checkWinner();
      if (result) {
        endGame(result);
      } else {
        document.getElementById('status').textContent = "Your turn (X)";
      }
    }

    // End game
    function endGame(result) {
      gameActive = false;
      const status = document.getElementById('status');
      if (result === 'X') status.textContent = "You win!";
      else if (result === 'O') status.textContent = "AI wins!";
      else status.textContent = "It's a tie!";
    }

    // Reset game
    function resetGame() {
      initGame();
    }

    // Start the game
    initGame();
