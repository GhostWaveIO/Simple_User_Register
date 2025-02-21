using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using System.IO;
using Cadastro.Models.Services.Application.IO.Files.Upload;

namespace Cadastro.Models.Account.Cadastro.Dados
{
    public partial class Dado_Cadastro {

    //Imagem
    [NotMapped]
    public IFormFile Imagem { get; set; }

    [NotMapped]
    public List<string> formatosImagemAceitos = new List<string>() { ".png", ".gif", ".jpg", ".jpeg", ".bmp", ".webp", ".tiff" };

    private void Preparar_Imagem() {

    }

    //#######################################################################################################################
    private void VerificarCriacao_Imagem() {
      if (this.Campo == null) throw new Exception("Campo não informado.");
      if (this.Imagem == null && !this.Campo.Required) return;
      List<string> formatosAceitos = formatosImagemAceitos;
      string extensao;

      if (this.Campo.Required && this.Imagem == null) throw new Exception($"O campo \"{this.Campo.Label}\" é obrigatório!");

      //Verifica se o campo sendo obrigatório, o arquivo foi enviado com êxito
        if (this.Campo.Required && (this.Imagem == null || (this.Imagem?.Length ?? 0) == 0)) throw new Exception($"Ocorreu um erro ao tentar enviar o arquivo do campo \"{this.Campo.Label}\"");

      //Verifica se ocorreu erro no envio
      if (this.Imagem != null && (Imagem?.Length ?? 0) == 0) throw new Exception("Erro ao enviar imagem!");

      //Verifica se o formato do arquivo é válido
      if (Imagem != null && (Imagem?.Length ?? 0) != 0) {
        extensao = Path.GetExtension(Imagem.FileName)?.Trim().ToLower();
        if(String.IsNullOrEmpty(extensao)) throw new Exception($"Formato de arquivo desconhecido no campo \"{this.Campo.Label}\".");
        if(!formatosAceitos.Contains(extensao)) throw new Exception($"Formato de arquivo inválido no campo \"{this.Campo.Label}\"<br/>" +
          $"Formatos aceitos ({new System.Text.StringBuilder().AppendJoin<string>(", ", formatosImagemAceitos)})");

        //Verifica se o tamanho do arquivo não ultrapassou o limite permitido
        if (Imagem.Length > 2621440) throw new Exception($"O arquivo é maior que o limite permitido ({(2621440 / 1024 / 1024).ToString("D2")} MB) no campo \"{this.Campo.Label}\"!");
      }
    }

    //#######################################################################################################################
    private void CorrigirCriacao_Imagem() {
      if (this.Imagem == null && !this.Campo.Required) return;
      this.Email = null;
      this.Numero = null;
      this.NumeroMonetario = null;
      this.Senha = "";
      this.Texto250 = null;
      this.TextoLongo = null;
    }

    //#######################################################################################################################
    private async Task Criar_Imagem() {
      if (this.Imagem == null && !this.Campo.Required) return;
      UploadImage upload = new UploadImage(this.Imagem, @$"Files\Cadastro\Imagens\{this.Campo.Campo_CadastroId}\", formatosImagemAceitos, 2621440);
      this.Local = upload.GerarNome();
      try {
        await upload.Upload();
      } catch (Exception err) {
        upload.DeleteFileError();
        throw new Exception("Ocorreu um erro ao tentar salvar a imagem", err);
      }
      
      await _context.DadosCadastro.AddAsync(this);
      await _context.SaveChangesAsync();
    }

    //#######################################################################################################################
    private void VerificarEdicao_Imagem() {
      if (this.Campo == null) throw new Exception("Campo não informado.");
      if(this.Imagem == null) return;
      List<string> formatosAceitos = formatosImagemAceitos;
      string extensao;

      if (this.Campo.Required && this.Imagem == null) throw new Exception($"O campo \"{this.Campo.Label}\" é obrigatório!");

      //Verifica se o campo sendo obrigatório, o arquivo foi enviado com êxito
      if (this.Campo.Required && (this.Imagem == null || (this.Imagem?.Length ?? 0) == 0)) throw new Exception($"Ocorreu um erro ao tentar enviar o arquivo do campo \"{this.Campo.Label}\"");

      //Verifica se ocorreu erro no envio
      if (this.Imagem != null && (Imagem?.Length ?? 0) == 0) throw new Exception("Erro ao enviar imagem!");

      //Verifica se o formato do arquivo é válido
      if (Imagem != null && (Imagem?.Length ?? 0) != 0) {
        extensao = Path.GetExtension(Imagem.FileName)?.Trim().ToLower();
        if (String.IsNullOrEmpty(extensao)) throw new Exception($"Formato de arquivo desconhecido no campo \"{this.Campo.Label}\".");
        if (!formatosAceitos.Contains(extensao)) throw new Exception($"Formato de arquivo inválido no campo \"{this.Campo.Label}\"<br/>" +
           $"Formatos aceitos ({new System.Text.StringBuilder().AppendJoin<string>(',', formatosImagemAceitos).ToString()})");

        //Verifica se o tamanho do arquivo não ultrapassou o limite permitido
        if (Imagem.Length > 2621440) throw new Exception($"O arquivo é maior que o limite permitido ({(2621440 / 1024 / 1024).ToString("D2")} MB) no campo \"{this.Campo.Label}\"!");
      }
    }

    //#######################################################################################################################
    private void CorrigirEdicao_Imagem() {
      if(this.Imagem == null) return;
      if((Imagem?.Length??0)==0) Imagem = null;
    }

    //#######################################################################################################################
    private void CopyToUpdate_Imagem(Dado_Cadastro dado) {
      if (this.Imagem == null) return;
      dado.Imagem = this.Imagem;
    }

    //#######################################################################################################################
    private async Task Update_Imagem() {
      if(this.Imagem == null) return;
      UploadImage upload = new UploadImage(this.Imagem, @$"Files\Cadastro\Imagens\{this.Campo.Campo_CadastroId}\", formatosImagemAceitos, 2621440);
      upload.SetImageToDele = this.Local;
      string novoNome = upload.GerarNome();
      try {
        await upload.Upload();
        this.Local = novoNome;
      } catch (Exception err) {
        upload.DeleteFileError();
        throw new Exception("Ocorreu um erro ao tentar salvar a imagem", err);
      }

      _context.DadosCadastro.Update(this);
      await _context.SaveChangesAsync();
    }

  }
}
