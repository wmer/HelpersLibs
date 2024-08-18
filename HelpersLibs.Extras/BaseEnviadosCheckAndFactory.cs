using CryptographyHelper;
using HelpersLib.Strings;
using HelpersLibs.Extras.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpersLibs.Extras;
public class BaseEnviadosCheckAndFactory {
    private TextInfo myTI;
    private Decryptor _decryptor;
    private readonly string key = "QG*¨@)*A13JkjjdD";
    private readonly string iniVector = "819GDDH@#@%3Hhds";

    public BaseEnviadosCheckAndFactory() {
        myTI = new CultureInfo("pt-BR", false).TextInfo;
        _decryptor = new Decryptor();
    }

    public (List<BaseEnviados>, int) CheckAndCreate(IEnumerable<DataRow> clientes) {
        var baseEnviados = new List<BaseEnviados>();
        var erros = 0;
        var provider = CultureInfo.CreateSpecificCulture("pt-BR");
        var styles = NumberStyles.Float;

        foreach (var contato in clientes) {
            try {
                var regua = contato["EMPRESA"].ToString()?.Trim();
                var canal = contato["CANAL"].ToString()?.Trim();
                var dtBase = contato["DT_BASE"].ToString()?.Trim();
                dtBase = StringHelper.RemoveSpecialCharacters(dtBase, "/-:.,");

                var cpf = contato["CPF_CNPJ_NUM"].ToString()?.Trim();

                if (!string.IsNullOrEmpty(cpf) && cpf.StartsWith("{")) {
                    cpf = cpf.Replace("{", "");
                    cpf = cpf.Replace("}", "");
                }

                cpf = TryDecrypt(cpf);

                var nomeCompleto = contato["NOME_COMPLETO"].ToString()?.Trim();

                nomeCompleto = TryDecrypt(nomeCompleto);

                nomeCompleto = myTI.ToTitleCase(nomeCompleto.ToLower());
                var nome = "";
                var sobrenome = "";

                if (!string.IsNullOrEmpty(nomeCompleto)) {
                    var nomeSobrenome = nomeCompleto.Split(' ');
                    nome = nomeSobrenome[0];

                    for (var j = 1; j < nomeSobrenome.Count(); j++) {
                        sobrenome += $"{nomeSobrenome[j]} ";
                    }
                } else {
                    nome = "Senhor(a)";
                }

                var numContrato = contato["CONTRATO"].ToString()?.Trim();
                numContrato = TryDecrypt(numContrato);

                var clienteId = contato["COD_CLIENTE"].ToString()?.Trim();
                clienteId = TryDecrypt(clienteId);

                var cidade = contato["CIDADE"].ToString()?.Trim();
                var estado = contato["ESTADO"].ToString()?.Trim();

                var numFatura = contato["FATURA"].ToString()?.Trim();
                numFatura = TryDecrypt(numFatura);

                var valorFatura = contato["VALOR"].ToString()?.Trim();
                var vencimentoFatura = StringHelper.RemoveSpecialCharacters(contato["DATA_VENC"].ToString()?.Trim(), "/-");
                var atraso = contato["ATRASO"].ToString()?.Trim();
                atraso = StringHelper.RemoveSpecialCharacters(atraso, "-");
                var atrasoCLiente = int.Parse(atraso);
                var messageBody = contato["MENSAGEM"].ToString().Trim();
                var taxajuros = contato.Table.Columns.Contains("TAXAJUROS") ? contato["TAXAJUROS"].ToString()?.Trim() : "";
                var link = contato["LINK"].ToString()?.Trim();
                link = TryDecrypt(link);
                var codBarras = contato["LINHA_DIG"].ToString()?.Trim();
                codBarras = TryDecrypt(codBarras);
                var tipoContato = myTI.ToTitleCase(contato["TELEFONE_TIPO"].ToString()?.Trim().ToLower()).Trim();
                var contatoVal = contato["TELEFONE_NUM"].ToString()?.Trim();
                contatoVal = TryDecrypt(contatoVal);
                contatoVal = StringHelper.GetOnlyPositiveNumbers(contatoVal)?.Trim();
                contatoVal = contatoVal.StartsWith("55") && contatoVal.Count() > 11 ? contatoVal.Substring(2) : contatoVal;

                if (canal == "E-mail") {
                    contatoVal = contato["EMAIL"].ToString().Trim();
                    contatoVal = TryDecrypt(contatoVal);
                }

                var particularidades = contato.Table.Columns.Contains("PARTICULAR_EMPRESA") ? contato["PARTICULAR_EMPRESA"].ToString()?.Trim() : null;
                DateTime.TryParse(vencimentoFatura, out DateTime vencFatura);

                if (string.IsNullOrEmpty(regua)) {
                    erros++;
                }


                if (string.IsNullOrEmpty(canal)) {
                    erros++;
                }

                var nomeRegua = StringHelper.RemoveSpecialCharacters(regua, ".-_-/áàãâéêèiîíìoôóòuûúùçÁÀÃÂÉÈÊIÍÌÎÓÔÒÚÙÛ");
                var numFat = StringHelper.RemoveSpecialCharacters(numFatura, ".-_-/");
                var canalStr = StringHelper.RemoveSpecialCharacters(canal, "--_");
                var numContr = StringHelper.RemoveSpecialCharacters(numContrato, ".-_-/");
                var idCli = StringHelper.RemoveSpecialCharacters(clienteId, ".-_-/");
                var linkFat = StringHelper.RemoveSpecialCharacters(link, "!*'();:@&=+$,/?#[]_.-~-%");
                var codB = StringHelper.RemoveSpecialCharacters(codBarras, "-.-");
                baseEnviados.Add(new BaseEnviados {
                    DTBase = DateTime.Parse(dtBase),
                    NomeRegua = nomeRegua,
                    Canal = canalStr,
                    Mensagem = messageBody,
                    NumeroFatura = numFat,
                    NumeroContrato = numContr,
                    IdCliente = idCli,
                    Contato = contatoVal,
                    Nome = nome,
                    SobreNome = sobrenome,
                    NomeCompleto = nomeCompleto,
                    CPFCNPJ = cpf,
                    ValorFatura = double.Parse(StringHelper.RemoveSpecialCharacters(valorFatura.Trim(), ".,").Replace('.', ','), styles, provider),
                    VencimentoFatura = vencFatura,
                    TaxaJuros = double.TryParse(StringHelper.RemoveSpecialCharacters(taxajuros.Trim(), ".,").Replace('.', ','), styles, provider, out double taxaJ) ? taxaJ : 0,
                    AtrasoCliente = atrasoCLiente,
                    Cidade = cidade,
                    Estado = estado,
                    LinkFatura = linkFat,
                    CodBarras = codB,
                    TipoContato = tipoContato,
                    ParticularidadesEmpresa = particularidades,
                    NumeroParcela = SearchVariable(particularidades, "NumeroParcela"),
                    CodebarBase64 = SearchVariable(particularidades, "CodebarBase64"),
                    BoletoBase64 = SearchVariable(particularidades, "BoletoBase64"),
                    PixBase64 = SearchVariable(particularidades, "PixBase64"),
                    CodigoPix = SearchVariable(particularidades, "CodigoPix"),
                    NumeroPedido = SearchVariable(particularidades, "NumeroPedido"),
                    Endereco = SearchVariable(particularidades, "Endereco"),
                    Matricula = SearchVariable(particularidades, "Matricula"),
                    DesContr = SearchVariable(particularidades, "des_contr"),
                    Credor = SearchVariable(particularidades, "Credor"),
                    NomeProduto = SearchVariable(particularidades, "NomeProduto"),
                    Prestacao = SearchVariable(particularidades, "Prestacao")
                });

            } catch (Exception e) {
                erros++;
            }
        }

        baseEnviados = baseEnviados.Where(x => !string.IsNullOrEmpty(x.Contato)).ToList();
        return (baseEnviados, erros);
    }

    private string? TryDecrypt(string? str) {
        if (!string.IsNullOrEmpty(str) && str.StartsWith("ECPD|")) {
            var tempStr = str.Replace("ECPD|", "");
            var strDecrypted = _decryptor.Decrypt(tempStr, key, iniVector);
            str = strDecrypted;
        }

        return str;
    }

    private static string SearchVariable(string particuliariades, string variable) {
        var search = "";

        if (!string.IsNullOrEmpty(particuliariades) && particuliariades.Contains("|||")) {
            var variFromParti = particuliariades.Split("|||");
            foreach (var vars in variFromParti) {
                search = ExtractVarValue(vars, variable);
                if (!string.IsNullOrEmpty(search)) {
                    break;
                }
            }
        } else if (!string.IsNullOrEmpty(particuliariades) && particuliariades.Contains(variable)) {
            search = ExtractVarValue(particuliariades, variable);
        }


        return search;
    }

    private static string ExtractVarValue(string str, string variable) {
        var search = "";
        if (str.Contains(variable)) {
            search = str.Replace($"{variable}:", "");

            if (search.Contains(";base64,")) {
                var resplacedSplit = search.Split(";base64,");
                search = resplacedSplit[1];
            }
            search = search.Trim();

            if (search.StartsWith("|")) {
                search = search[2..];
            }
        }

        return search;
    }
}