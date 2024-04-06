import {Component, HostListener} from '@angular/core';
import {NgForOf} from "@angular/common";
import {MatGridList, MatGridTile} from "@angular/material/grid-list";
import {MatButton} from "@angular/material/button";
import {GameService} from "../../services/game.service";
import {Cell} from "../../models/board";

@Component({
  selector: 'app-board',
  standalone: true,
  imports: [
    NgForOf,
    MatGridList,
    MatGridTile,
    MatButton
  ],
  templateUrl: './board.component.html',
  styleUrl: './board.component.css'
})
export class BoardComponent {
  activeField?: Cell;
  grid: Cell[][] | undefined;

  constructor(private gameService: GameService) {
    // Initialize the grid with 0 rows and 0 cols

    this.gameService.onLetterChange().subscribe(letterChange => {
      console.log('Letter changed', letterChange);

      if (this.grid)
        this.grid[letterChange.row][letterChange.col].value = letterChange.value ?? null;
    });

    this.gameService.onBoardChange().subscribe(boardChange => {
      this.grid = boardChange.cells;
    });

    this.gameService.onKicked().subscribe(() => {
      this.grid = undefined;
      console.error('You have been kicked from the game');
    });
  }

  selectCell(cell: Cell) {
    if (this.activeField === cell) {
      this.activeField = undefined;
      return;
    }
    this.activeField = cell;
  }

  commitWord() {
    this.activeField = undefined;
    if (this.grid)
      this.grid.forEach(row => row.forEach(cell => cell.committedValue = cell.value));
  }

  @HostListener('window:keydown.arrowDown')
  moveDown() {
    if (!this.grid || !this.activeField)
      return;

    const newRow = Math.min(this.activeField.row + 1, 14);
    this.activeField = this.grid[newRow][this.activeField.col];
  }

  @HostListener('window:keydown.arrowUp')
  moveUp() {
    if (!this.grid || !this.activeField)
      return;

    const newRow = Math.max(this.activeField.row - 1, 0);
    this.activeField = this.grid[newRow][this.activeField.col];
  }

  @HostListener('window:keydown.arrowLeft')
  moveLeft() {
    if (!this.grid || !this.activeField)
      return;

    const newCol = Math.max(this.activeField.col - 1, 0);
    this.activeField = this.grid[this.activeField.row][newCol];
  }

  @HostListener('window:keydown.arrowRight')
  moveRight() {
    if (!this.grid || !this.activeField)
      return;

    const newCol = Math.min(this.activeField.col + 1, 14);
    this.activeField = this.grid[this.activeField.row][newCol];
  }

  @HostListener('window:keydown', ['$event'])
  onKeyDown(event: KeyboardEvent) {
    if (!this.activeField)
      return;

    // If the key is an alphabetical letter, save it to a variable named "letter", otherwise return
    const letterMatch = event.key.match(/^[A-Z]$/i);
    const backspaceMatch = event.key.match(/^Backspace$/i);

    if (backspaceMatch) {
      this.activeField.value = null;
      this.gameService.placeLetter(this.activeField.row, this.activeField.col, null);
      return;
    }

    const letter = letterMatch ? letterMatch[0].toUpperCase() : null;

    if (!letter) {
      return;
    }

    this.activeField.value = letter;
    this.gameService.placeLetter(this.activeField.row, this.activeField.col, letter);
  }
}
