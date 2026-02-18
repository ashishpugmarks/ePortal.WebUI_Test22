<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" indent="yes" encoding="utf-8"/>

  <xsl:template match="/Menus">
    <ul>
      <xsl:call-template name="MenuListing" />
    </ul>
  </xsl:template>
  <xsl:template name="MenuListing">
    <xsl:apply-templates select="Menu" >
      <xsl:sort select="DisplayOrder" data-type="number" order="ascending"/>
    </xsl:apply-templates>
  </xsl:template>
  <xsl:template match="Menu">
    <li>
      <a>
        <xsl:attribute name="href">
          <xsl:value-of select="URL"/>
        </xsl:attribute>
        <xsl:value-of select="Description"/>
      </a>
      <xsl:if test="count(Menu) > 0">
        <ul>
          <xsl:call-template name="MenuListing" />
        </ul>
      </xsl:if>
    </li>
  </xsl:template>
</xsl:stylesheet>

