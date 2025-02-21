using System;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Cadastro.Models.Services.Application.IO.Files.Upload;

namespace Cadastro.Models.Account.Cadastro.Dados
{
    public partial class Dado_Cadastro {    

    [NotMapped]
    public IFormFile Documento { get; set; }

    //#######################################################################################################################
    private void Preparar_Documento() {

    }

    //#######################################################################################################################
    private void VerificarCriacao_Documento() {
      if(this.Campo == null) throw new Exception("Campo não informado.");
      if (this.Documento == null && !this.Campo.Required) return;
      List<string> formatosAceitos;
      string extensao = null;
	  
			if(this.Documento == null && this.Campo.Required)
				throw new Exception($"O campo \"{this.Campo.Label}\" é obrigatório!");
			else if(this.Documento == null)
				return;

      //Verifica se o campo sendo obrigatório, o arquivo foi enviado com êxito
      if ((this.Documento?.Length??0) == 0) throw new Exception($"Ocorreu um erro ao tentar enviar o arquivo do campo \"{this.Campo.Label}\"");

      //Verifica se o formato do arquivo é válido
      if (Documento != null && (Documento?.Length??0) != 0) {
        extensao = Path.GetExtension(Documento.FileName)?.Trim().ToLower();
        if (String.IsNullOrEmpty(extensao)) throw new Exception($"Formato de arquivo desconhecido no campo \"{this.Campo.Label}\".");
        formatosAceitos = this.Campo.FormatosArquivo?.Replace(" ", "").Split(',').ToList();
        if (!formatosAceitos.Contains(extensao)) throw new Exception($"Formato de arquivo inválido no campo \"{this.Campo.Label}\"<br/>" +
          $"Formatos aceitos ({this.Campo.FormatosArquivo})");

        //Verifica se o tamanho do arquivo não ultrapassou o limite permitido
        if (Documento.Length > 5242880) throw new Exception($"O arquivo é maior que o limite permitido ({(5242880 / 1024 / 1024).ToString("D2")} MB) no campo \"{this.Campo.Label}\"!");
      }
        
    }

    //#######################################################################################################################
    private void CorrigirCriacao_Documento() {
      if (this.Documento == null && !this.Campo.Required) return;
      this.Email = null;
      this.Numero = null;
      this.NumeroMonetario = null;
      this.Senha = "";
      this.Texto250 = null;
      this.TextoLongo = null;
    }

    //#######################################################################################################################
    private async Task Criar_Documento() {
      if(this.Documento == null && !this.Campo.Required) return;
      UploadDocument upload = new UploadDocument(this.Documento, Path.Combine("Files", "Cadastro", "Documentos", this.Campo.Campo_CadastroId.ToString()), this.Campo.FormatosArquivo.Replace(" ", "").Split(',').ToList(), 5242880);
      this.Local = upload.GerarNome();
      try {
        await upload.Upload();
      } catch (Exception err) {
        upload.DeleteFileError();
        throw new Exception("Ocorreu um erro ao tentar salvar o documento", err);
      }

      await _context.DadosCadastro.AddAsync(this);
      await _context.SaveChangesAsync();
    }

    //#######################################################################################################################
    private void VerificarEdicao_Documento() {
      if (this.Campo == null) throw new Exception("Campo não informado.");
      if (this.Documento == null) return;
      List<string> formatosAceitos;
      string extensao = null;

      if(this.Documento == null && this.Campo.Required)
				throw new Exception($"O campo \"{this.Campo.Label}\" é obrigatório!");
			else if(this.Documento == null)
				return;

      //Verifica se o campo sendo obrigatório, o arquivo foi enviado com êxito
      if ((this.Documento?.Length??0) == 0) throw new Exception($"Ocorreu um erro ao tentar enviar o arquivo do campo \"{this.Campo.Label}\"");

      //Verifica se o formato do arquivo é válido
      if (Documento != null && (Documento?.Length ?? 0) != 0) {
        extensao = Path.GetExtension(Documento.FileName)?.Trim().ToLower();
        if (String.IsNullOrEmpty(extensao)) throw new Exception($"Formato de arquivo desconhecido no campo \"{this.Campo.Label}\".");
        formatosAceitos = this.Campo.FormatosArquivo?.Replace(" ", "").Split(',').ToList();
        if (!formatosAceitos.Contains(extensao)) throw new Exception($"Formato de arquivo inválido no campo \"{this.Campo.Label}\"<br/>" +
          $"Formatos aceitos ({this.Campo.FormatosArquivo})");

        //Verifica se o tamanho do arquivo não ultrapassou o limite permitido
        if (Documento.Length > 5242880) throw new Exception($"O arquivo é maior que o limite permitido ({(5242880 / 1024 / 1024).ToString("D2")} MB) no campo \"{this.Campo.Label}\"!");
      }
    }

    //#######################################################################################################################
    private void CorrigirEdicao_Documento() {
      if (this.Documento == null) return;
      if ((Documento?.Length ?? 0) == 0) Documento = null;
    }

    //#######################################################################################################################
    private void CopyToUpdate_Documento(Dado_Cadastro dado) {
      if (this.Documento == null) return;
      dado.Documento = this.Documento;
    }

    //#######################################################################################################################
    private async Task Update_Documento() {
      if (this.Documento == null) return;
      UploadDocument upload = new UploadDocument(this.Documento, @$"Files\Cadastro\Documentos\{this.Campo.Campo_CadastroId}\", this.Campo.FormatosArquivo.Replace(" ", "").Split(',').ToList(), 5242880);
      upload.SetDocumentToDele = this.Local;
      string novoNome = upload.GerarNome();
      try {
        await upload.Upload();
        this.Local = novoNome;
      } catch (Exception err) {
        upload.DeleteFileError();
        throw new Exception("Ocorreu um erro ao tentar salvar o documento", err);
      }

      _context.DadosCadastro.Update(this);
      await _context.SaveChangesAsync();
    }

  }
}
