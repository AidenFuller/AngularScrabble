import {Component, HostListener} from '@angular/core';
import {NgForOf} from "@angular/common";
import {MatGridList, MatGridTile} from "@angular/material/grid-list";
import {MatButton} from "@angular/material/button";
import {GameService} from "../../services/game.service";

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
  activeField?: ScrabbleField;
  grid: ScrabbleField[][];

  constructor(private gameService: GameService) {
    this.grid = Array.from({length: 15}, () => Array.from({length: 15}));
    // Initialize the grid with X as the value for each field
    for (let row = 0; row < 15; row++) {
      for (let col = 0; col < 15; col++) {
        this.grid[row][col] = {
          value: null,
          row: row,
          col: col,
          multiplier: getMultiplier(row, col),
          committedValue: null
        };
      }
    }

    this.gameService.onLetterChange().subscribe(letterChange => {
      this.grid[letterChange.row][letterChange.col].value = letterChange.value;
    })
  }

  selectCell(cell: ScrabbleField) {
    if (this.activeField === cell) {
      this.activeField = undefined;
      return;
    }
    this.activeField = cell;
  }

  commitWord() {
    this.gameService.joinGame('TEST', 'Player 1');

    this.activeField = undefined;
    this.grid.forEach(row => row.forEach(cell => cell.committedValue = cell.value));
  }

  @HostListener('window:keydown.arrowDown')
  moveDown() {
    if (!this.activeField)
      return;

    const newRow = Math.min(this.activeField.row + 1, 14);
    this.activeField = this.grid[newRow][this.activeField.col];
  }

  @HostListener('window:keydown.arrowUp')
  moveUp() {
    if (!this.activeField)
      return;

    const newRow = Math.max(this.activeField.row - 1, 0);
    this.activeField = this.grid[newRow][this.activeField.col];
  }

  @HostListener('window:keydown.arrowLeft')
  moveLeft() {
    if (!this.activeField)
      return;

    const newCol = Math.max(this.activeField.col - 1, 0);
    this.activeField = this.grid[this.activeField.row][newCol];
  }

  @HostListener('window:keydown.arrowRight')
  moveRight() {
    if (!this.activeField)
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

const tripleWordFlatIndexes = [0, 7, 14, 105, 119, 210, 217, 224];
const doubleWordFlatIndexes = [16, 28, 32, 42, 48, 56, 64, 70, 112, 154, 160, 168, 176, 182, 192, 196, 208];
const tripleLetterFlatIndexes = [20, 24, 76, 80, 84, 88, 136, 140, 144, 148, 200, 204];
const doubleLetterFlatIndexes = [3, 11, 36, 38, 45, 52, 59, 92, 96, 98, 102, 108, 116, 122, 126, 128, 132, 165, 172, 179, 186, 188, 213, 221];

function getMultiplier(row: number,  col: number): Multiplier {
  const flatIndex = row * 15 + col;
  if (tripleWordFlatIndexes.includes(flatIndex)) {
    return 'TW';
  }
  if (doubleWordFlatIndexes.includes(flatIndex)) {
    return 'DW';
  }
  if (tripleLetterFlatIndexes.includes(flatIndex)) {
    return 'TL';
  }
  if (doubleLetterFlatIndexes.includes(flatIndex)) {
    return 'DL';
  }
  return null;
}

export interface ScrabbleField {
  value: string | null;
  committedValue: string | null;
  multiplier?: Multiplier;
  row: number;
  col: number;
}

export type Multiplier = 'DL' | 'TL' | 'DW' | 'TW' | null;
