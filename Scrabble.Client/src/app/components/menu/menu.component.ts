import { Component } from '@angular/core';
import { MatFormField, MatLabel } from "@angular/material/form-field";
import { MatInput } from "@angular/material/input";
import { SessionService } from "../../services/session.service";
import { GameService } from "../../services/game.service";
import { MatButton } from "@angular/material/button";
import { FormsModule } from "@angular/forms";

@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [
    MatLabel,
    MatFormField,
    MatInput,
    MatButton,
    FormsModule
  ],
  templateUrl: './menu.component.html',
  styleUrl: './menu.component.css'
})
export class MenuComponent {
  sessionKey: string | null;
  playerName: string | null;
  latestScore: number;

  constructor(private sessionService: SessionService, private gameService: GameService) {
    this.sessionKey = sessionService.getSessionKey();
    this.playerName = sessionService.getPlayerName();
    this.latestScore = 0;

    this.gameService.onScoreChange().subscribe(score => {
      this.latestScore = score;
      console.log('Score changed', score);
    })
  }

  private hasSessionKey(): boolean {
    return this.sessionKey !== null;
  }

  protected onJoinClick() {
    if (!this.sessionKey || !this.playerName) {
      console.error('Session key and player name are required');
      return;
    }

    this.joinSession(this.sessionKey, this.playerName);
  }

  private joinSession(sessionKey: string, playerName: string) {
    if (!sessionKey || !playerName) {
      console.error('Session key and player name are required');
      return;
    }

    this.sessionService.setSessionKey(sessionKey);
    this.sessionService.setPlayerName(playerName);
    this.gameService.joinGame(sessionKey, playerName);
  }

  protected commitWord(): void {
    this.gameService.commitWord();
  }

  protected startGame(): void {
    this.gameService.startGame();
  }
}
