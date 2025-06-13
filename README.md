# 📘 Projeto - Gerador e Validador de Código de Barras com AZURE FUNCTIONS e Azure Service Bus

> Projeto desenvolvido para consumo de duas Azure Functions:
> - Uma função para geração de código de barras (porta 7146)
> - Uma função para validação do código de barras (porta 7023)

---

## 📝 Descrição

A aplicação permite ao usuário informar uma data de vencimento e um valor em reais para gerar um código de barras e sua representação visual em imagem (base64).  
Após a geração, é possível validar o código por meio de uma API e destacar visualmente se o código é válido (`verde`) ou inválido (`vermelho`).

---

## 📷 Evidências Visuais

### 1. Envio de dados para geração (`/barcode-generate` via Postman)
![image](https://github.com/user-attachments/assets/f0689059-2574-461a-96ec-3486549e8f9f)


---

### 2. Validação via Postman (`/barcode-validate`)
![image](https://github.com/user-attachments/assets/dcf37f29-05a5-4c53-82cd-b07da6e99446)

---

### 3. Requisitos de entrada informados no chat
![image](https://github.com/user-attachments/assets/029409fb-df83-4e45-b6ec-b28fb3c67b08)


---

### 4. Interface: formulário com campos de entrada e botão de geração
![image](https://github.com/user-attachments/assets/f9350a45-95f5-4d21-a795-5a99c338485c)


---

### 5. Requisitos da ação de validação após geração
![image](https://github.com/user-attachments/assets/b0eef86a-a76b-434e-95d2-274d34bff913)

---

### 6. Erro de CORS e 404 no preflight
![image](https://github.com/user-attachments/assets/a576499f-757c-40d4-b8fe-33b6db6469f1)

---

### 7. Configuração de CORS no Visual Studio (`--cors "*"` e porta)
![image](https://github.com/user-attachments/assets/e2703ee9-a305-4ac1-b44b-2774893adac3)

---

### 8. Validação com sucesso no navegador
![image](https://github.com/user-attachments/assets/73e2a013-06d7-4c5f-bdd4-5391c2df7929)



---

### 9. Interface mostrando código inválido (estilizado em vermelho)

![image](https://github.com/user-attachments/assets/96404b9d-dc9c-41da-a824-b9d2376c3969)

#### A cor do barcode estava vermelha, apesar de a validação ter tido sucesso no preview do DevTools

---

### 10. Resposta JSON com `valido: true`
![image](https://github.com/user-attachments/assets/ba44b2a8-33d2-4a91-ae8f-1223fe58f7c5)

---

### 11. Prompts para o GitHub Copilot e resposta do GitHub Copilot
![image](https://github.com/user-attachments/assets/cb0bdb4e-47c0-4fc0-a519-1ea7c0d35743)
![image](https://github.com/user-attachments/assets/22116146-d3ac-4b27-8d65-8b8e2afe993c)

![image](https://github.com/user-attachments/assets/052c1f83-ba30-4ad5-8f77-1304325ba823)

---

### 12. Interface final com código validado em verde
![image](https://github.com/user-attachments/assets/7420f61c-6faf-40e2-b97b-7f51090cf685)

---

### 13. Mensagens salvas na fila do Azure Service Bus
![image](https://github.com/user-attachments/assets/5f254d6f-212c-4478-b943-0b20e7787971)

---

## 💻 Tecnologias Utilizadas

- .NET 8 com Azure Functions (C#) (isolated worker)
- HTML5, CSS e JavaScript (interface)
- GitHub Copilot (auxílio no desenvolvimento)
- Visual Studio (backend/API)
- VS Code com extensão Live Server (frontend/UI)
- Azure Service Bus
- Postman

---

## 🧾 Notas Finais

Este README segue o modelo padrão utilizado por mim em projetos para Desafios na DIO, adaptado conforme instruções fornecidas no decorrer do projeto.
