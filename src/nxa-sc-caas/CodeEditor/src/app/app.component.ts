import { Component, ViewChild, ElementRef } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { MonacoEditorComponent } from './monaco-editor/monaco-editor.component';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  private headers: HttpHeaders;

  private readonly url=`${window.location.origin}/api/Compiler`;
  constructor(private readonly http: HttpClient) {}
  initCode = `
    // Type smart contract code here...
  `;
  resultCode=`
    // Compile result will be shown here
  `;

  public compileCode(): void {
    var token=(document.getElementById("tokenInput") as HTMLInputElement).value;
    this.headers = new HttpHeaders({ 'Content-Type': 'application/json', 'Token': token});
    var createCompilerTask:any = 
    {
      systemOwnerAddress: (document.getElementById("systemOwnerAddress") as HTMLInputElement).value,
      contractAuthorAddress: (document.getElementById("contractAuthorAddress") as HTMLInputElement).value,
      contractAuthorName: (document.getElementById("contractAuthorName") as HTMLInputElement).value,
      contractAuthorEmail: (document.getElementById("contractAuthorEmail") as HTMLInputElement).value,
      contractName: (document.getElementById("contractName") as HTMLInputElement).value,
      contractDescription: (document.getElementById("contractDescription") as HTMLInputElement).value,
      contractSymbol: (document.getElementById("contractSymbol") as HTMLInputElement).value,
      contractInitialCoins: (document.getElementById("contractInitialCoins") as HTMLInputElement).value,
      contractDecimals: (document.getElementById("contractDecimals") as HTMLInputElement).value,
      contractSource: this.initCode
    };
    console.log(`compiling code -> ${this.initCode}`);
    
    this.http.put(this.url, createCompilerTask, { headers: this.headers })
    .subscribe(resp=> this.resultCode = JSON.stringify(resp, null, 2));
  }

  clearCode(): void {
    console.log('clearing the code');
    this.initCode = '';
  }
}
