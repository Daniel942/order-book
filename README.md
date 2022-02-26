# Order Book project
Project that retrieves order book data from Bitstamp and displays it in a market depth chart. It stores retrieved data in an audit log, which can be used to display previously retrieved data. In addition, user can check the current BTC price based on the requested units they wish to purchase.

## Instructions
Open `order-book/order-book-backend/order-book-backend` and run `dotnet run`

Open `order-book/order-book-frontend`.
Run `yarn install` and then `yarn start`

## Disclaimer
Make sure that the frontend is running on port 3000, since backend has an origin exception with that port configured. Feel free to change or run it differently.