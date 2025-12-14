# Product Order Demo

A full-stack demo application showcasing the integration of Nuxt.js 4 (frontend) and .NET Core 9.0 (backend) to demonstrate modern web development skills with AI-powered development workflow.

## 🚀 Tech Stack

### Frontend
- **Nuxt.js 4** - Vue 3 framework with TypeScript
- **Server-Side Rendering (SSR)**
- **Composition API**
- **Modern CSS3**

### Backend
- **.NET Core 9.0** - Latest version of ASP.NET Core
- **Web API** with RESTful endpoints
- **OpenAPI/Swagger** documentation
- **CORS enabled** for cross-origin requests

## 📋 Features

- **Product Catalog** - Browse available products with images, prices, and stock information
- **Shopping Cart** - Add products to cart with quantity management
- **Order Management** - Create and manage orders with status tracking
- **RESTful API** - Clean API architecture with proper HTTP methods
- **Responsive Design** - Mobile-friendly user interface

## 🛠️ Project Structure

```
product-order-demo/
├── ProductOrderApi/          # .NET Core 9.0 Backend
│   ├── Controllers/          # API Controllers
│   │   ├── ProductsController.cs
│   │   └── OrdersController.cs
│   ├── Models/               # Data Models
│   │   ├── Product.cs
│   │   └── Order.cs
│   └── Program.cs            # Application entry point
│
└── product-order-ui/         # Nuxt.js Frontend
    ├── pages/                # Vue Pages
    │   ├── index.vue         # Home page
    │   ├── products.vue      # Product catalog
    │   ├── checkout.vue      # Checkout page
    │   └── orders.vue        # Order management
    └── nuxt.config.ts        # Nuxt configuration
```

## 🚦 Getting Started

### Prerequisites

- Node.js 18+ and npm
- .NET SDK 9.0+

### Backend Setup

1. Navigate to the API directory:
```bash
cd ProductOrderApi
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Run the API:
```bash
dotnet run
```

The API will start on `http://localhost:5000` (or `https://localhost:5001` for HTTPS).

### Frontend Setup

1. Navigate to the UI directory:
```bash
cd product-order-ui
```

2. Install dependencies:
```bash
npm install
```

3. Run the development server:
```bash
npm run dev
```

The application will be available at `http://localhost:3000`.

## 📚 API Endpoints

### Products
- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create a new product
- `PUT /api/products/{id}` - Update a product
- `DELETE /api/products/{id}` - Delete a product

### Orders
- `GET /api/orders` - Get all orders
- `GET /api/orders/{id}` - Get order by ID
- `POST /api/orders` - Create a new order
- `PUT /api/orders/{id}` - Update an order
- `PATCH /api/orders/{id}/status` - Update order status
- `DELETE /api/orders/{id}` - Delete an order

## 🎨 UI Pages

- **Home (/)** - Landing page with navigation
- **/products** - Product catalog with shopping cart
- **/checkout** - Checkout form for placing orders
- **/orders** - Order management with status updates

## 🔧 Configuration

### Backend (ProductOrderApi/appsettings.json)
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

### Frontend (product-order-ui/nuxt.config.ts)
```typescript
export default defineNuxtConfig({
  runtimeConfig: {
    public: {
      apiBase: 'http://localhost:5000'
    }
  }
})
```

## 🧪 Testing the Application

1. Start both the backend API and frontend application
2. Navigate to `http://localhost:3000`
3. Browse products and add items to cart
4. Proceed to checkout and place an order
5. View and manage orders in the orders page

## 🤖 AI Development Showcase

This project demonstrates:
- Rapid full-stack application development
- Modern framework integration (Nuxt.js + .NET Core)
- RESTful API design patterns
- State management in Vue 3
- Responsive UI design
- TypeScript usage for type safety

## 📝 License

This is a demo project for showcasing development skills.

## 👤 Author

**Rahul Lore**
- Showcasing AI-powered development capabilities
- Modern full-stack development expertise