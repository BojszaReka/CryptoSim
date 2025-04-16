# CryptoSim
Assignment, advanced programming techniques  subject: cryptocurrency buy-sell simulator backend

# Known Issues
/will be fixed later on/
Some endpoints return the following error:
"The instance of entity type 'xy' cannot be tracked because another instance with the same key value for {'Id'} is already being tracked."
<p></p>
But after sending the request again it returns with success and completes all the neccessary process.

# API Endpoints

### 🔐 Users

| Method | Endpoint                     | Description                              |
|--------|------------------------------|------------------------------------------|
| POST   | `/api/users/register`        | Register a new user                      |
| POST   | `/api/users/login`           | Login and retrieve user ID               |
| GET    | `/api/users/{UserId}`        | Get user details                         |
| PUT    | `/api/users/{UserId}`        | Update user password                     |
| DELETE | `/api/users/{UserId}`        | Delete a user                            |

---

### 💰 Wallets

| Method | Endpoint                     | Description                              |
|--------|------------------------------|------------------------------------------|
| GET    | `/api/wallet/{UserId}`       | Get wallet by user ID                    |
| PUT    | `/api/wallet`                | Update wallet balance                    |
| DELETE | `/api/wallet/{UserId}`       | Delete a user's wallet                   |

---

### 📊 Crypto Currencies

| Method | Endpoint                     | Description                              |
|--------|------------------------------|------------------------------------------|
| GET    | `/api/cryptos`               | List all available cryptocurrencies      |
| GET    | `/api/cryptos/{CryptoId}`    | Get details of a specific crypto         |
| POST   | `/api/cryptos`               | Create a new cryptocurrency              |
| DELETE | `/api/cryptos/{CryptoId}`    | Delete a cryptocurrency                  |

---

### 🔄 Crypto Prices

| Method | Endpoint                              | Description                              |
|--------|----------------------------------------|------------------------------------------|
| PUT    | `/api/crypto/price`                   | Update the price of a crypto             |
| GET    | `/api/crypto/price/history/{CryptoId}`| Get price history of a crypto            |

---

### 📈 Trades

| Method | Endpoint                             | Description                              |
|--------|--------------------------------------|------------------------------------------|
| POST   | `/api/trade/buy`                     | Buy cryptocurrency                       |
| POST   | `/api/trade/sell`                    | Sell cryptocurrency                      |
| GET    | `/api/trade/portfolio/{UserId}`      | Get user’s portfolio                     |

---

### 📃 Transactions


| Method | Endpoint                                                    | Description                              |
|--------|-------------------------------------------------------------|------------------------------------------|
| GET    | `/api/transactions/GetUserTransactions/{UserId}`           | Get all transactions by user             |
| GET    | `/api/transactions/GetTransationDetails/{TransactionId}`   | Get details of a transaction             |

---

### 💹 Profit

| Method | Endpoint                              | Description                              |
|--------|----------------------------------------|------------------------------------------|
| GET    | `/api/profit/profit/{UserId}`          | Get total profit for a user              |
| GET    | `/api/profit/profit/detail/{UserId}`   | Get detailed profit breakdown            |
