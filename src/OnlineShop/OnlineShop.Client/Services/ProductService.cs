﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Script.Serialization;
using OnlineShop.DTO;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using OnlineShop.Client.Exceptions;

namespace OnlineShop.Client.Services
{
    class ProductService : IProductService, IDisposable
    {
        private readonly HttpClient client;
        private readonly JavaScriptSerializer serializer;

        public ProductService(IAuthenticationService authService)
        {
            client = new HttpClient();
            serializer = new JavaScriptSerializer();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authService.AuthenticationToken.Login + ":" + authService.AuthenticationToken.Token);
        }

        public async Task<ProductDto> CreateAsync(ProductDto product)
        {
            string jsonRequest = serializer.Serialize(product);
            string requestUri = Properties.Resources.UrlToServer + "Product/Create";
            try
            {
                var responseMessage = await client.PostAsync(requestUri, new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
                if (responseMessage.IsSuccessStatusCode)
                {
                    string jsonResult = await responseMessage.Content.ReadAsStringAsync();
                    product = serializer.Deserialize<ProductDto>(jsonResult);
                }
                else
                {
                    product = null;
                }

                return product;
            }
            catch (HttpRequestException ex)
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                {
                    throw new NoInternetConnectionException("No internet connection, please try again later.");
                }
                throw ex;
            }
        }

        public async Task DeleteAsync(int id)
        {
            string jsonRequest = serializer.Serialize(id.ToString());
            string requestUri = Properties.Resources.UrlToServer + "Product/Delete";
            HttpRequestMessage request = new HttpRequestMessage
            {
                Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json"),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(requestUri)
            };

            try
            {
                var responseMessage = await client.SendAsync(request);
            }
            catch (HttpRequestException ex)
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                {
                    throw new NoInternetConnectionException("No internet connection, please try again later.");
                }
                throw ex;
            }
            
        }

        public async Task<ProductDto> GetProductAsync(int id)
        {
            ProductDto product = null;
            string requestUri = Properties.Resources.UrlToServer + "Product/GetProduct/" + id;
            try
            {
                var responceMessage = await client.GetAsync(requestUri);
                if (responceMessage.IsSuccessStatusCode)
                {
                    string jsonResult = await responceMessage.Content.ReadAsStringAsync();
                    product = serializer.Deserialize<ProductDto>(jsonResult);
                }
                return product;
            }
            catch (HttpRequestException ex)
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                {
                    throw new NoInternetConnectionException("No internet connection, please try again later.");
                }
                throw ex;
            }
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            IEnumerable<ProductDto> products = new ProductDto[0];
            string requestUri = Properties.Resources.UrlToServer + "Product/GetAllProducts";
            try
            {
                var responceMessage = await client.GetAsync(requestUri);
                if (responceMessage.IsSuccessStatusCode)
                {
                    string jsonResult = await responceMessage.Content.ReadAsStringAsync();
                    products = serializer.Deserialize<IEnumerable<ProductDto>>(jsonResult);
                }

                return products;
            }
            catch (HttpRequestException ex)
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                {
                    throw new NoInternetConnectionException("No internet connection, please try again later.");
                }
                throw ex;
            }
        }

        public async Task UpdateAsync(ProductDto product)
        {
            string jsonRequest = serializer.Serialize(product);
            string requestUri = Properties.Resources.UrlToServer + "Product/Update";
            try
            {
                var responseMessage = await client.PutAsync(requestUri, new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
            }
            catch (HttpRequestException ex)
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                {
                    throw new NoInternetConnectionException("No internet connection, please try again later.");
                }
                throw ex;
            }
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}
