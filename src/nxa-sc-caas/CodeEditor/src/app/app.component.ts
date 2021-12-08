import { Component } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from './../environments/environment';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  private static readonly initCodeDefault = `
    //
    // Type smart contract code here...
    //
  `;

  private static readonly resultCodeDefault = `
    //
    // Compile results
    //
  `;

  public initCode = AppComponent.initCodeDefault;
  public resultCode = AppComponent.resultCodeDefault;

  constructor(private readonly httpClient: HttpClient) {
  }

  public async compileCode(event: any): Promise<void> {
    const apiUrl = `${window.location.origin}/api/Compiler`;
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Token': environment.token
    });

    const createCompilerTask: { [key: string]: string } = {
      compilerTaskType: (document.getElementById('compilerTaskType') as HTMLInputElement).value,
      systemOwnerAddress: (document.getElementById('systemOwnerAddress') as HTMLInputElement).value,
      contractAuthorAddress: (document.getElementById('contractAuthorAddress') as HTMLInputElement).value,
      contractAuthorName: (document.getElementById('contractAuthorName') as HTMLInputElement).value,
      contractAuthorEmail: (document.getElementById('contractAuthorEmail') as HTMLInputElement).value,
      contractName: (document.getElementById('contractName') as HTMLInputElement).value,
      contractDescription: (document.getElementById('contractDescription') as HTMLInputElement).value,
      contractSymbol: (document.getElementById('contractSymbol') as HTMLInputElement).value,
      contractInitialCoins: (document.getElementById('contractInitialCoins') as HTMLInputElement).value,
      contractDecimals: (document.getElementById('contractDecimals') as HTMLInputElement).value,
      contractSource: this.initCode
    };
    console.log(`compiling code -> ${this.initCode} with: ${JSON.stringify(createCompilerTask)}`);

    event.target.disabled = true;

    try {
      const resp = await this.httpClient.put(apiUrl, createCompilerTask, { headers: headers }).toPromise();
      this.resultCode = JSON.stringify(resp, null, 2);
    } catch (e) {
      console.error(e);
    } finally {
      event.target.disabled = false;
    }
  }

  public clearCode(event: any): void {
    this.resultCode = AppComponent.resultCodeDefault;
  }
}
