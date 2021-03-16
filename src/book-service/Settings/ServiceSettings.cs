namespace BookService.Settings
{
    /// <summary>
    /// This properties is used on the Appsettings.Development.json
    /// </summary>
    public class ServiceSettings
    {
        /// <summary>
        /// Consul service identification ID.
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// DNS/Localhost where the service is hosted.
        /// </summary>
        public string ServiceHost { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ServicePort { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ServiceDiscoveryAddress { get; set; }
    }
}
