import { Injectable } from '@angular/core';
import { CookieService } from "ngx-cookie-service";

const SESSION_COOKIE_NAME = 'scrabble-session-key';
const PLAYER_COOKIE_NAME = 'scrabble-player-name';

@Injectable({
  providedIn: 'root'
})
export class SessionService {

  constructor(private cookieService: CookieService) {
  }

  public hasSessionKey(): boolean {
    return this.cookieService.check(SESSION_COOKIE_NAME);
  }

  public hasPlayerName(): boolean {
    return this.cookieService.check(PLAYER_COOKIE_NAME);
  }

  public getSessionKey(): string | null {
    if (!this.hasSessionKey()) return null;

    return this.cookieService.get(SESSION_COOKIE_NAME);
  }

  public getPlayerName(): string | null {
    if (!this.hasPlayerName()) return null;

    return this.cookieService.get(PLAYER_COOKIE_NAME);
  }

  public setSessionKey(sessionKey: string) {
    this.cookieService.set(SESSION_COOKIE_NAME, sessionKey);
  }

  public setPlayerName(playerName: string) {
    this.cookieService.set(PLAYER_COOKIE_NAME, playerName);
  }
}
